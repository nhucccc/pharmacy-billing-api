using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class DispensationRepository : BaseRepository<Dispensation>, IDispensationRepository
{
    public DispensationRepository(PharmacyDbContext context) : base(context) { }

    public async Task<Dispensation?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.Include(d => d.Items).ThenInclude(i => i.Medicine)
                       .FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<Dispensation?> GetByPrescriptionIdAsync(Guid prescriptionId, CancellationToken ct = default)
        => await _dbSet.Include(d => d.Items)
                       .FirstOrDefaultAsync(d => d.PrescriptionId == prescriptionId, ct);

    public async Task<IReadOnlyList<Dispensation>> GetByPatientIdAsync(Guid patientId,
        int page, int pageSize, CancellationToken ct = default)
        => await _dbSet.Where(d => d.PatientId == patientId)
                       .OrderByDescending(d => d.CreatedAt)
                       .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IReadOnlyList<Dispensation>> GetByStatusAsync(DispensationStatus status, CancellationToken ct = default)
        => await _dbSet.Where(d => d.Status == status).OrderBy(d => d.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Dispensation>> GetTodayDispensationsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.Include(d => d.Items)
                           .Where(d => d.CreatedAt >= today)
                           .OrderByDescending(d => d.CreatedAt).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Dispensation>> SearchAsync(string? keyword, DispensationStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct = default)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            q = q.Where(d => d.PatientName.Contains(keyword) ||
                              d.DoctorName.Contains(keyword) ||
                              d.DispensationCode.Contains(keyword));
        if (status.HasValue) q = q.Where(d => d.Status == status.Value);
        if (patientId.HasValue) q = q.Where(d => d.PatientId == patientId.Value);
        if (from.HasValue) q = q.Where(d => d.CreatedAt >= from.Value);
        if (to.HasValue) q = q.Where(d => d.CreatedAt <= to.Value);
        return await q.Include(d => d.Items)
                      .OrderByDescending(d => d.CreatedAt)
                      .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(string? keyword, DispensationStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            q = q.Where(d => d.PatientName.Contains(keyword) ||
                              d.DoctorName.Contains(keyword) ||
                              d.DispensationCode.Contains(keyword));
        if (status.HasValue) q = q.Where(d => d.Status == status.Value);
        if (patientId.HasValue) q = q.Where(d => d.PatientId == patientId.Value);
        if (from.HasValue) q = q.Where(d => d.CreatedAt >= from.Value);
        if (to.HasValue) q = q.Where(d => d.CreatedAt <= to.Value);
        return await q.CountAsync(ct);
    }
}
