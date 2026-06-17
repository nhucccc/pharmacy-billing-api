using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Domain.Interfaces;

public interface IStockTransactionRepository : IRepository<StockTransaction>
{
    Task<IReadOnlyList<StockTransaction>> GetByMedicineIdAsync(Guid medicineId,
        int page = 1, int pageSize = 50, CancellationToken ct = default);
    Task<IReadOnlyList<StockTransaction>> GetByDateRangeAsync(DateTime from, DateTime to,
        CancellationToken ct = default);
}
