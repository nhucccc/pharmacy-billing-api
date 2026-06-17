using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Domain.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<Invoice?> GetByCodeAsync(string invoiceCode, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByPatientIdAsync(Guid patientId, int page = 1,
        int pageSize = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> SearchAsync(string? keyword, InvoiceStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(string? keyword, InvoiceStatus? status,
        Guid? patientId, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<Dictionary<string, decimal>> GetRevenueByDateAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
