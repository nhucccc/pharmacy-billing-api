namespace PharmacyBilling.Application.DTOs.User;

public record UpdateProfileRequest(
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    DateTime? DateOfBirth,
    string? Gender,
    string? InsuranceNumber
);
