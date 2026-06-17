using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class DispensationConfiguration : IEntityTypeConfiguration<Dispensation>
{
    public void Configure(EntityTypeBuilder<Dispensation> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DispensationCode).IsRequired().HasMaxLength(30);
        builder.Property(d => d.PatientName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.DoctorName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Diagnosis).HasMaxLength(500);
        builder.Property(d => d.Notes).HasMaxLength(1000);
        builder.Property(d => d.Status).HasConversion<int>();

        builder.HasIndex(d => d.DispensationCode).IsUnique();
        builder.HasIndex(d => d.PrescriptionId).IsUnique(); // One dispensation per prescription
        builder.HasIndex(d => d.PatientId);
        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasMany(d => d.Items)
               .WithOne(i => i.Dispensation)
               .HasForeignKey(i => i.DispensationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Invoice)
               .WithOne(i => i.Dispensation)
               .HasForeignKey<Invoice>(i => i.DispensationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
