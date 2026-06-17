using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Domain.Interfaces;

public interface IDispensationRepository : IRepository<Dispensation>
{
    Task<Dispensation?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<Dispensation?> GetByPrescriptionIdAsync(Guid prescriptionId, CancellationToken ct = default);
    Task<IReadOnlyList<Dispensation>> GetByPatientIdAsync(Guid patientId, int page = 1,
        int pageSize = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Dispensation>> GetByStatusAsync(DispensationStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Dispensation>> GetTodayDispensationsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Dispensation>> SearchAsync(string? keyword, DispensationStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(string? keyword, DispensationStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, CancellationToken ct = default);
}
