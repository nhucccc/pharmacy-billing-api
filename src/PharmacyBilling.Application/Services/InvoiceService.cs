using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.Common.Models;
using PharmacyBilling.Application.DTOs.Invoice;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.Application.Services;

public class InvoiceService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IUnitOfWork uow, ILogger<InvoiceService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<PagedResult<InvoiceDto>>> GetAllAsync(string? keyword,
        InvoiceStatus? status, Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize,
        CancellationToken ct = default)
    {
        var items = await _uow.Invoices.SearchAsync(keyword, status, patientId, from, to, page, pageSize, ct);
        var total = await _uow.Invoices.GetTotalCountAsync(keyword, status, patientId, from, to, ct);
        return Result<PagedResult<InvoiceDto>>.Success(
            PagedResult<InvoiceDto>.Create(items.Select(MapToDto).ToList(), total, page, pageSize));
    }

    public async Task<Result<InvoiceDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var inv = await _uow.Invoices.GetWithItemsAsync(id, ct);
        if (inv == null) return Result<InvoiceDto>.NotFound("Invoice not found.");
        return Result<InvoiceDto>.Success(MapToDto(inv));
    }

    public async Task<Result<InvoiceDto>> CreateAsync(CreateInvoiceRequest request, Guid createdBy, CancellationToken ct = default)
    {
        // Get medicine fee from dispensation if provided
        decimal medicineFee = 0;
        Dispensation? dispensation = null;

        if (request.DispensationId.HasValue)
        {
            dispensation = await _uow.Dispensations.GetWithItemsAsync(request.DispensationId.Value, ct);
            if (dispensation == null)
                return Result<InvoiceDto>.NotFound("Dispensation not found.");
            if (dispensation.Status != DispensationStatus.Dispensed)
                return Result<InvoiceDto>.Failure("Dispensation must be completed before creating invoice.");
            medicineFee = dispensation.CalculateTotalMedicineCost();
        }

        var invoice = Invoice.Create(
            request.PatientId, request.PatientName, request.ExaminationFee,
            medicineFee, request.PatientCode, request.InsuranceNumber,
            request.DoctorName, request.DispensationId, request.AppointmentId,
            request.OtherFees, request.Notes);

        if (request.DiscountAmount > 0)
            invoice.ApplyDiscount(request.DiscountAmount, "Manual discount");

        if (request.InsuranceCoverage > 0)
            invoice.ApplyInsurance(request.InsuranceCoverage);

        // Add invoice items from dispensation
        if (dispensation != null)
        {
            foreach (var item in dispensation.Items)
            {
                var invItem = InvoiceItem.Create(invoice.Id, item.MedicineName, "MEDICINE",
                    item.Quantity, item.UnitPrice, item.Dosage);
                invoice.InvoiceItems.Add(invItem);
            }
        }

        // Add examination fee item
        if (request.ExaminationFee > 0)
        {
            var examItem = InvoiceItem.Create(invoice.Id, "Examination Fee", "EXAMINATION",
                1, request.ExaminationFee);
            invoice.InvoiceItems.Add(examItem);
        }

        await _uow.Invoices.AddAsync(invoice, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Invoice created: {Code} total: {Amount}", invoice.InvoiceCode, invoice.TotalAmount);
        var result = await _uow.Invoices.GetWithItemsAsync(invoice.Id, ct);
        return Result<InvoiceDto>.Success(MapToDto(result!));
    }

    public async Task<Result<InvoiceDto>> PayAsync(Guid id, PayInvoiceRequest request, Guid collectedBy, CancellationToken ct = default)
    {
        var invoice = await _uow.Invoices.GetWithItemsAsync(id, ct);
        if (invoice == null) return Result<InvoiceDto>.NotFound("Invoice not found.");

        invoice.MarkAsPaid(request.PaymentMethod, collectedBy);
        _uow.Invoices.Update(invoice);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Invoice paid: {Code}, Amount: {Amount}, Method: {Method}",
            invoice.InvoiceCode, invoice.TotalAmount, request.PaymentMethod);
        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<InvoiceDto>> CancelAsync(Guid id, string reason, CancellationToken ct = default)
    {
        var invoice = await _uow.Invoices.GetWithItemsAsync(id, ct);
        if (invoice == null) return Result<InvoiceDto>.NotFound("Invoice not found.");
        invoice.Cancel(reason);
        _uow.Invoices.Update(invoice);
        await _uow.SaveChangesAsync(ct);
        return Result<InvoiceDto>.Success(MapToDto(invoice));
    }

    public async Task<Result<object>> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var total = await _uow.Invoices.GetTotalRevenueAsync(from, to, ct);
        var byDate = await _uow.Invoices.GetRevenueByDateAsync(from, to, ct);
        return Result<object>.Success(new { TotalRevenue = total, ByDate = byDate });
    }

    private static InvoiceDto MapToDto(Invoice inv) => new(
        inv.Id, inv.InvoiceCode, inv.PatientId, inv.PatientName,
        inv.PatientCode, inv.InsuranceNumber, inv.DoctorName,
        inv.DispensationId, inv.AppointmentId,
        inv.ExaminationFee, inv.MedicineFee, inv.OtherFees,
        inv.DiscountAmount, inv.InsuranceCoverage, inv.TotalAmount,
        inv.Status, inv.Status.ToString(),
        inv.PaymentMethod, inv.PaymentMethod?.ToString(),
        inv.PaidAt, inv.Notes,
        inv.InvoiceItems.Select(i => new InvoiceItemDto(
            i.Id, i.ItemName, i.ItemType, i.Quantity, i.UnitPrice, i.SubTotal, i.Note)).ToList(),
        inv.CreatedAt);
}
