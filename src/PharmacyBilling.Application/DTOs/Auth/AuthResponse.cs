using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Auth;
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    Guid UserId,
    string Username,
    string FullName,
    string Email,
    UserRole Role,
    string RoleName
);
