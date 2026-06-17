using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MedicineCode).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.ActiveIngredient).IsRequired().HasMaxLength(300);
        builder.Property(m => m.Manufacturer).HasMaxLength(200);
        builder.Property(m => m.CountryOfOrigin).HasMaxLength(100);
        builder.Property(m => m.Unit).HasConversion<int>();
        builder.Property(m => m.UnitDescription).HasMaxLength(100);
        builder.Property(m => m.Category).HasMaxLength(100);
        builder.Property(m => m.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(m => m.ImportPrice).HasColumnType("decimal(18,2)");
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.SideEffects).HasMaxLength(1000);
        builder.Property(m => m.StorageConditions).HasMaxLength(500);

        builder.HasIndex(m => m.MedicineCode).IsUnique();
        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.Category);
        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.HasMany(m => m.DispensationItems)
               .WithOne(di => di.Medicine)
               .HasForeignKey(di => di.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.StockTransactions)
               .WithOne(st => st.Medicine)
               .HasForeignKey(st => st.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
