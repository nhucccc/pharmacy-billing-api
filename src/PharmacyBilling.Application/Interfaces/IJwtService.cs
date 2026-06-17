using PharmacyBilling.Domain.Entities;

namespace PharmacyBilling.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    (bool isValid, string? username) ValidateAccessToken(string token);
    DateTime GetAccessTokenExpiry();
}
