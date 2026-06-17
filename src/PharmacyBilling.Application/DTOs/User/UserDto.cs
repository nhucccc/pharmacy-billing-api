using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    string RoleName,
    bool IsActive,
    string? PatientCode,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? InsuranceNumber,
    string? AvatarUrl,
    DateTime CreatedAt
);
