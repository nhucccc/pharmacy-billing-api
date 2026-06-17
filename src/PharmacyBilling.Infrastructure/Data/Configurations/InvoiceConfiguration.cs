using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.InvoiceCode).IsRequired().HasMaxLength(30);
        builder.Property(i => i.PatientName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.PatientCode).HasMaxLength(20);
        builder.Property(i => i.InsuranceNumber).HasMaxLength(50);
        builder.Property(i => i.DoctorName).HasMaxLength(100);
        builder.Property(i => i.ExaminationFee).HasColumnType("decimal(18,2)");
        builder.Property(i => i.MedicineFee).HasColumnType("decimal(18,2)");
        builder.Property(i => i.OtherFees).HasColumnType("decimal(18,2)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.InsuranceCoverage).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Status).HasConversion<int>();
        builder.Property(i => i.PaymentMethod).HasConversion<int?>();
        builder.Property(i => i.Notes).HasMaxLength(1000);
        builder.Ignore(i => i.TotalAmount);

        builder.HasIndex(i => i.InvoiceCode).IsUnique();
        builder.HasIndex(i => i.PatientId);
        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasMany(i => i.InvoiceItems)
               .WithOne(ii => ii.Invoice)
               .HasForeignKey(ii => ii.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
