using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(PharmacyDbContext context) : base(context) { }

    public async Task<Invoice?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.Include(i => i.InvoiceItems)
                       .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Invoice?> GetByCodeAsync(string invoiceCode, CancellationToken ct = default)
        => await _dbSet.Include(i => i.InvoiceItems)
                       .FirstOrDefaultAsync(i => i.InvoiceCode == invoiceCode, ct);

    public async Task<IReadOnlyList<Invoice>> GetByPatientIdAsync(Guid patientId,
        int page, int pageSize, CancellationToken ct = default)
        => await _dbSet.Where(i => i.PatientId == patientId)
                       .OrderByDescending(i => i.CreatedAt)
                       .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken ct = default)
        => await _dbSet.Where(i => i.Status == status).OrderBy(i => i.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Invoice>> SearchAsync(string? keyword, InvoiceStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct = default)
    {
        var q = _dbSet.Include(i => i.InvoiceItems).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            q = q.Where(i => i.PatientName.Contains(keyword) ||
                              i.InvoiceCode.Contains(keyword) ||
                              (i.PatientCode != null && i.PatientCode.Contains(keyword)));
        if (status.HasValue) q = q.Where(i => i.Status == status.Value);
        if (patientId.HasValue) q = q.Where(i => i.PatientId == patientId.Value);
        if (from.HasValue) q = q.Where(i => i.CreatedAt >= from.Value);
        if (to.HasValue) q = q.Where(i => i.CreatedAt <= to.Value);
        return await q.OrderByDescending(i => i.CreatedAt)
                      .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(string? keyword, InvoiceStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            q = q.Where(i => i.PatientName.Contains(keyword) ||
                              i.InvoiceCode.Contains(keyword) ||
                              (i.PatientCode != null && i.PatientCode.Contains(keyword)));
        if (status.HasValue) q = q.Where(i => i.Status == status.Value);
        if (patientId.HasValue) q = q.Where(i => i.PatientId == patientId.Value);
        if (from.HasValue) q = q.Where(i => i.CreatedAt >= from.Value);
        if (to.HasValue) q = q.Where(i => i.CreatedAt <= to.Value);
        return await q.CountAsync(ct);
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet.Where(i => i.Status == InvoiceStatus.Paid &&
                                   i.PaidAt >= from && i.PaidAt <= to)
                       .SumAsync(i => i.ExaminationFee + i.MedicineFee + i.OtherFees - i.DiscountAmount - i.InsuranceCoverage, ct);

    public async Task<Dictionary<string, decimal>> GetRevenueByDateAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var data = await _dbSet
            .Where(i => i.Status == InvoiceStatus.Paid && i.PaidAt >= from && i.PaidAt <= to)
            .GroupBy(i => i.PaidAt!.Value.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(i => i.ExaminationFee + i.MedicineFee + i.OtherFees - i.DiscountAmount - i.InsuranceCoverage)
            })
            .ToListAsync(ct);

        return data.ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Revenue);
    }
}
