using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.PatientCode).HasMaxLength(20);
        builder.Property(u => u.Gender).HasMaxLength(10);
        builder.Property(u => u.Address).HasMaxLength(300);
        builder.Property(u => u.InsuranceNumber).HasMaxLength(50);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.RefreshToken).HasMaxLength(500);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Seed admin user (password: Admin@123)
        builder.HasData(new
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Username = "admin",
            PasswordHash = "$2a$11$rBnG3rGi7MJqaGJpZY.5.eR8KmhG9jLZ/qY9Y.0.u6V8L/L2q3aCC", // Admin@123
            FullName = "System Administrator",
            Email = "admin@pharmacy.com",
            PhoneNumber = (string?)null,
            Role = UserRole.Admin,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = (DateTime?)null,
            RefreshToken = (string?)null,
            RefreshTokenExpiryTime = (DateTime?)null,
            PatientCode = (string?)null,
            DateOfBirth = (DateTime?)null,
            Gender = (string?)null,
            Address = (string?)null,
            InsuranceNumber = (string?)null,
            AvatarUrl = (string?)null
        });
    }
}
