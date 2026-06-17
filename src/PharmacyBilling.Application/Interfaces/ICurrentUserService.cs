using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Username { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}
