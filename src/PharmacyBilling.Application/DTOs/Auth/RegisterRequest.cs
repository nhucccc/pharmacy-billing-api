using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Auth;
public record RegisterRequest(
    string Username,
    string Password,
    string ConfirmPassword,
    string FullName,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? InsuranceNumber
);
