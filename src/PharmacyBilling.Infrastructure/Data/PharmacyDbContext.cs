using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data;

public class PharmacyDbContext : DbContext
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Dispensation> Dispensations => Set<Dispensation>();
    public DbSet<DispensationItem> DispensationItems => Set<DispensationItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Clear events after save
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());
        return result;
    }
}
