using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default);
    Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
    Task<IReadOnlyList<User>> SearchAsync(string? keyword, UserRole? role,
        bool? isActive, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(string? keyword, UserRole? role, bool? isActive, CancellationToken ct = default);
}
