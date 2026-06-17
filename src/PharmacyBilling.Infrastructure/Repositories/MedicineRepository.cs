using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class MedicineRepository : BaseRepository<Medicine>, IMedicineRepository
{
    public MedicineRepository(PharmacyDbContext context) : base(context) { }

    public async Task<Medicine?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(m => m.MedicineCode == code, ct);

    public async Task<IReadOnlyList<Medicine>> GetLowStockMedicinesAsync(CancellationToken ct = default)
        => await _dbSet.Where(m => m.IsActive && m.StockQuantity <= m.MinimumStock)
                       .OrderBy(m => m.StockQuantity).ToListAsync(ct);

    public async Task<IReadOnlyList<Medicine>> SearchAsync(string keyword, string? category,
        bool? activeOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(m => m.Name.Contains(keyword) ||
                                     m.ActiveIngredient.Contains(keyword) ||
                                     m.MedicineCode.Contains(keyword));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(m => m.Category == category);
        if (activeOnly.HasValue)
            query = query.Where(m => m.IsActive == activeOnly.Value);

        return await query.OrderBy(m => m.Name)
                          .Skip((page - 1) * pageSize).Take(pageSize)
                          .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(string? keyword, string? category,
        bool? activeOnly, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(m => m.Name.Contains(keyword) ||
                                     m.ActiveIngredient.Contains(keyword) ||
                                     m.MedicineCode.Contains(keyword));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(m => m.Category == category);
        if (activeOnly.HasValue)
            query = query.Where(m => m.IsActive == activeOnly.Value);
        return await query.CountAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default)
        => await _dbSet.Where(m => m.Category != null && m.IsActive)
                       .Select(m => m.Category!).Distinct().OrderBy(c => c).ToListAsync(ct);

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId, CancellationToken ct = default)
    {
        var query = _dbSet.Where(m => m.MedicineCode == code);
        if (excludeId.HasValue) query = query.Where(m => m.Id != excludeId.Value);
        return !await query.AnyAsync(ct);
    }
}
