namespace PharmacyBilling.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMedicineRepository Medicines { get; }
    IUserRepository Users { get; }
    IDispensationRepository Dispensations { get; }
    IInvoiceRepository Invoices { get; }
    IStockTransactionRepository StockTransactions { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
