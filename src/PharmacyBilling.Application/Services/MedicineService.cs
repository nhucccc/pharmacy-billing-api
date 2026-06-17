using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.Common.Models;
using PharmacyBilling.Application.DTOs.Medicine;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.Application.Services;

public class MedicineService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MedicineService> _logger;

    public MedicineService(IUnitOfWork uow, ILogger<MedicineService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<PagedResult<MedicineDto>>> GetAllAsync(string? keyword, string? category,
        bool? activeOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var items = await _uow.Medicines.SearchAsync(keyword ?? "", category, activeOnly, page, pageSize, ct);
        var total = await _uow.Medicines.GetTotalCountAsync(keyword, category, activeOnly, ct);
        var dtos = items.Select(MapToDto).ToList();
        return Result<PagedResult<MedicineDto>>.Success(PagedResult<MedicineDto>.Create(dtos, total, page, pageSize));
    }

    public async Task<Result<MedicineDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var medicine = await _uow.Medicines.GetByIdAsync(id, ct);
        if (medicine == null) return Result<MedicineDto>.NotFound("Medicine not found.");
        return Result<MedicineDto>.Success(MapToDto(medicine));
    }

    public async Task<Result<MedicineDto>> CreateAsync(CreateMedicineRequest request, CancellationToken ct = default)
    {
        var medicine = Medicine.Create(
            request.Name, request.ActiveIngredient, request.Unit,
            request.UnitPrice, request.ImportPrice, request.InitialStock,
            request.RequiresPrescription, request.Manufacturer,
            request.Category, request.Description, request.MinimumStock);

        await _uow.Medicines.AddAsync(medicine, ct);

        if (request.InitialStock > 0)
        {
            var tx = StockTransaction.Create(medicine.Id, "IN", request.InitialStock, 0,
                request.InitialStock, request.ImportPrice, null, "Initial stock", null);
            await _uow.StockTransactions.AddAsync(tx, ct);
        }

        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Medicine created: {Name} (Code: {Code})", medicine.Name, medicine.MedicineCode);
        return Result<MedicineDto>.Success(MapToDto(medicine));
    }

    public async Task<Result<MedicineDto>> UpdateAsync(Guid id, UpdateMedicineRequest request, CancellationToken ct = default)
    {
        var medicine = await _uow.Medicines.GetByIdAsync(id, ct);
        if (medicine == null) return Result<MedicineDto>.NotFound("Medicine not found.");

        medicine.UpdateInfo(request.Name, request.ActiveIngredient, request.Unit,
            request.UnitPrice, request.ImportPrice, request.Manufacturer,
            request.Category, request.Description, request.SideEffects,
            request.StorageConditions, request.RequiresPrescription, request.MinimumStock);

        _uow.Medicines.Update(medicine);
        await _uow.SaveChangesAsync(ct);
        return Result<MedicineDto>.Success(MapToDto(medicine));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var medicine = await _uow.Medicines.GetByIdAsync(id, ct);
        if (medicine == null) return Result.NotFound("Medicine not found.");
        medicine.Deactivate();
        _uow.Medicines.Update(medicine);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<MedicineDto>> ImportStockAsync(Guid id, StockImportRequest request, Guid importedBy, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var medicine = await _uow.Medicines.GetByIdAsync(id, ct);
            if (medicine == null) return Result<MedicineDto>.NotFound("Medicine not found.");

            var stockBefore = medicine.StockQuantity;
            medicine.AddStock(request.Quantity, request.Note);

            var tx = StockTransaction.Create(medicine.Id, "IN", request.Quantity,
                stockBefore, medicine.StockQuantity, medicine.ImportPrice,
                null, request.Note, importedBy);
            await _uow.StockTransactions.AddAsync(tx, ct);

            _uow.Medicines.Update(medicine);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Stock imported: {Name} +{Qty} units", medicine.Name, request.Quantity);
            return Result<MedicineDto>.Success(MapToDto(medicine));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error importing stock for medicine {Id}", id);
            return Result<MedicineDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<MedicineDto>> AdjustStockAsync(Guid id, StockAdjustRequest request, Guid adjustedBy, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var medicine = await _uow.Medicines.GetByIdAsync(id, ct);
            if (medicine == null) return Result<MedicineDto>.NotFound("Medicine not found.");

            var stockBefore = medicine.StockQuantity;
            medicine.AdjustStock(request.NewQuantity, request.Reason);

            var diff = request.NewQuantity - stockBefore;
            var txType = diff >= 0 ? "ADJUST_IN" : "ADJUST_OUT";
            var tx = StockTransaction.Create(medicine.Id, txType, Math.Abs(diff),
                stockBefore, medicine.StockQuantity, medicine.ImportPrice,
                null, request.Reason, adjustedBy);
            await _uow.StockTransactions.AddAsync(tx, ct);

            _uow.Medicines.Update(medicine);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return Result<MedicineDto>.Success(MapToDto(medicine));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            return Result<MedicineDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<IReadOnlyList<MedicineDto>>> GetLowStockAsync(CancellationToken ct = default)
    {
        var items = await _uow.Medicines.GetLowStockMedicinesAsync(ct);
        return Result<IReadOnlyList<MedicineDto>>.Success(items.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<string>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var cats = await _uow.Medicines.GetCategoriesAsync(ct);
        return Result<IReadOnlyList<string>>.Success(cats);
    }

    public async Task<Result<object>> GetStockHistoryAsync(Guid medicineId, int page, int pageSize, CancellationToken ct = default)
    {
        var transactions = await _uow.StockTransactions.GetByMedicineIdAsync(medicineId, page, pageSize, ct);
        var result = transactions.Select(t => new
        {
            t.Id,
            t.TransactionType,
            t.Quantity,
            t.StockBefore,
            t.StockAfter,
            t.Note,
            t.CreatedAt
        }).ToList();
        return Result<object>.Success(result);
    }

    private static MedicineDto MapToDto(Medicine m) => new(
        m.Id, m.MedicineCode, m.Name, m.ActiveIngredient,
        m.Manufacturer, m.CountryOfOrigin, m.Unit, m.Unit.ToString(),
        m.UnitDescription, m.Category, m.UnitPrice, m.ImportPrice,
        m.StockQuantity, m.MinimumStock, m.IsLowStock(),
        m.Description, m.SideEffects, m.StorageConditions,
        m.RequiresPrescription, m.IsActive, m.ExpiryDate, m.CreatedAt);
}
