using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class DispensationItemConfiguration : IEntityTypeConfiguration<DispensationItem>
{
    public void Configure(EntityTypeBuilder<DispensationItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.MedicineName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.ActiveIngredient).IsRequired().HasMaxLength(300);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Dosage).HasMaxLength(200);
        builder.Property(i => i.Usage).HasMaxLength(500);
        builder.Ignore(i => i.SubTotal);
    }
}
