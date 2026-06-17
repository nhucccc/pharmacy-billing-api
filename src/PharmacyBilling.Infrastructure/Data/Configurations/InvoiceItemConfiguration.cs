using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ItemName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.ItemType).IsRequired().HasMaxLength(50);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Note).HasMaxLength(500);
        builder.Ignore(i => i.SubTotal);
    }
}
