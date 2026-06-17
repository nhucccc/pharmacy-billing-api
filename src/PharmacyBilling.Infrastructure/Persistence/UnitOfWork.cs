using Microsoft.EntityFrameworkCore.Storage;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;
using PharmacyBilling.Infrastructure.Repositories;

namespace PharmacyBilling.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PharmacyDbContext _context;
    private IDbContextTransaction? _transaction;

    public IMedicineRepository Medicines { get; }
    public IUserRepository Users { get; }
    public IDispensationRepository Dispensations { get; }
    public IInvoiceRepository Invoices { get; }
    public IStockTransactionRepository StockTransactions { get; }

    public UnitOfWork(PharmacyDbContext context)
    {
        _context = context;
        Medicines = new MedicineRepository(context);
        Users = new UserRepository(context);
        Dispensations = new DispensationRepository(context);
        Invoices = new InvoiceRepository(context);
        StockTransactions = new StockTransactionRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
