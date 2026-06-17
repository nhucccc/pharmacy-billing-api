using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class StockTransactionRepository : BaseRepository<StockTransaction>, IStockTransactionRepository
{
    public StockTransactionRepository(PharmacyDbContext context) : base(context) { }

    public async Task<IReadOnlyList<StockTransaction>> GetByMedicineIdAsync(Guid medicineId,
        int page, int pageSize, CancellationToken ct = default)
        => await _dbSet.Where(s => s.MedicineId == medicineId)
                       .OrderByDescending(s => s.CreatedAt)
                       .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IReadOnlyList<StockTransaction>> GetByDateRangeAsync(DateTime from, DateTime to,
        CancellationToken ct = default)
        => await _dbSet.Include(s => s.Medicine)
                       .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
                       .OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
}
