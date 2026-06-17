using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.TransactionType).IsRequired().HasMaxLength(20);
        builder.Property(s => s.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ReferenceId).HasMaxLength(100);
        builder.Property(s => s.Note).HasMaxLength(500);
        builder.HasIndex(s => s.MedicineId);
        builder.HasIndex(s => s.CreatedAt);
    }
}
