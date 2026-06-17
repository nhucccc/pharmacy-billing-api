using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Domain.Interfaces;

public interface IMedicineRepository : IRepository<Medicine>
{
    Task<Medicine?> GetByCodeAsync(string medicineCode, CancellationToken ct = default);
    Task<IReadOnlyList<Medicine>> GetLowStockMedicinesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Medicine>> SearchAsync(string keyword, string? category = null,
        bool? activeOnly = true, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(string? keyword = null, string? category = null,
        bool? activeOnly = true, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default);
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}
