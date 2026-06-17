using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.Common.Models;
using PharmacyBilling.Application.DTOs.Dispensation;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.Application.Services;

public class DispensationService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DispensationService> _logger;

    public DispensationService(IUnitOfWork uow, ILogger<DispensationService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<PagedResult<DispensationDto>>> GetAllAsync(string? keyword,
        DispensationStatus? status, Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize,
        CancellationToken ct = default)
    {
        var items = await _uow.Dispensations.SearchAsync(keyword, status, patientId, from, to, page, pageSize, ct);
        var total = await _uow.Dispensations.GetTotalCountAsync(keyword, status, patientId, from, to, ct);
        return Result<PagedResult<DispensationDto>>.Success(
            PagedResult<DispensationDto>.Create(items.Select(MapToDto).ToList(), total, page, pageSize));
    }

    public async Task<Result<DispensationDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var d = await _uow.Dispensations.GetWithItemsAsync(id, ct);
        if (d == null) return Result<DispensationDto>.NotFound("Dispensation not found.");
        return Result<DispensationDto>.Success(MapToDto(d));
    }

    public async Task<Result<DispensationDto>> CreateFromPrescriptionAsync(
        CreateDispensationRequest request, CancellationToken ct = default)
    {
        // Check if prescription already processed
        var existing = await _uow.Dispensations.GetByPrescriptionIdAsync(request.PrescriptionId, ct);
        if (existing != null)
            return Result<DispensationDto>.Conflict("This prescription has already been processed.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var dispensation = Dispensation.Create(
                request.PrescriptionId, request.PatientId, request.DoctorId,
                request.PatientName, request.DoctorName, request.Diagnosis,
                request.AppointmentId, request.Notes);

            await _uow.Dispensations.AddAsync(dispensation, ct);

            // Validate và tạo items
            foreach (var itemReq in request.Items)
            {
                var medicine = await _uow.Medicines.GetByIdAsync(itemReq.MedicineId, ct);
                if (medicine == null)
                    throw new InvalidOperationException($"Medicine {itemReq.MedicineId} not found.");

                if (!medicine.IsActive)
                    throw new InvalidOperationException($"Medicine '{medicine.Name}' is no longer active.");

                var item = DispensationItem.Create(
                    dispensation.Id, medicine.Id, medicine.Name, medicine.ActiveIngredient,
                    itemReq.Quantity, medicine.UnitPrice, itemReq.Dosage, itemReq.Usage, itemReq.DurationDays);

                dispensation.AddItem(item);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Dispensation created: {Code} for prescription {PrescriptionId}",
                dispensation.DispensationCode, request.PrescriptionId);

            var result = await _uow.Dispensations.GetWithItemsAsync(dispensation.Id, ct);
            return Result<DispensationDto>.Success(MapToDto(result!));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error creating dispensation for prescription {PrescriptionId}", request.PrescriptionId);
            return Result<DispensationDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<DispensationDto>> ProcessDispensationAsync(Guid id, Guid processedBy, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var dispensation = await _uow.Dispensations.GetWithItemsAsync(id, ct);
            if (dispensation == null) return Result<DispensationDto>.NotFound("Dispensation not found.");

            dispensation.StartProcessing();

            // Deduct stock for each item
            foreach (var item in dispensation.Items)
            {
                var medicine = await _uow.Medicines.GetByIdAsync(item.MedicineId, ct);
                if (medicine == null) throw new InvalidOperationException($"Medicine not found: {item.MedicineId}");

                var stockBefore = medicine.StockQuantity;
                medicine.DeductStock(item.Quantity);

                var tx = StockTransaction.Create(medicine.Id, "OUT", item.Quantity,
                    stockBefore, medicine.StockQuantity, item.UnitPrice,
                    dispensation.Id.ToString(), $"Dispensed for {dispensation.DispensationCode}", processedBy);

                await _uow.StockTransactions.AddAsync(tx, ct);
                _uow.Medicines.Update(medicine);
            }

            dispensation.Complete(processedBy);
            _uow.Dispensations.Update(dispensation);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Dispensation completed: {Code}", dispensation.DispensationCode);
            return Result<DispensationDto>.Success(MapToDto(dispensation));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            return Result<DispensationDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<DispensationDto>> CancelAsync(Guid id, string reason, CancellationToken ct = default)
    {
        var dispensation = await _uow.Dispensations.GetWithItemsAsync(id, ct);
        if (dispensation == null) return Result<DispensationDto>.NotFound("Dispensation not found.");
        dispensation.Cancel(reason);
        _uow.Dispensations.Update(dispensation);
        await _uow.SaveChangesAsync(ct);
        return Result<DispensationDto>.Success(MapToDto(dispensation));
    }

    private static DispensationDto MapToDto(Dispensation d) => new(
        d.Id, d.DispensationCode, d.PrescriptionId, d.PatientId,
        d.PatientName, d.DoctorName, d.Diagnosis,
        d.Status, d.Status.ToString(),
        d.Notes, d.DispensedAt,
        d.Items.Select(i => new DispensationItemDto(
            i.Id, i.MedicineId, i.MedicineName, i.ActiveIngredient,
            i.Quantity, i.UnitPrice, i.SubTotal,
            i.Dosage, i.Usage, i.DurationDays)).ToList(),
        d.CalculateTotalMedicineCost(), d.CreatedAt);
}
