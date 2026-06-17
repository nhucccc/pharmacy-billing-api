using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(PharmacyDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default)
        => await _dbSet.Where(u => u.Role == role).ToListAsync(ct);

    public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId, CancellationToken ct = default)
    {
        var query = _dbSet.IgnoreQueryFilters().Where(u => u.Username == username);
        if (excludeId.HasValue) query = query.Where(u => u.Id != excludeId.Value);
        return !await query.AnyAsync(ct);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId, CancellationToken ct = default)
    {
        var query = _dbSet.IgnoreQueryFilters().Where(u => u.Email == email);
        if (excludeId.HasValue) query = query.Where(u => u.Id != excludeId.Value);
        return !await query.AnyAsync(ct);
    }

    public async Task<IReadOnlyList<User>> SearchAsync(string? keyword, UserRole? role,
        bool? isActive, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(u => u.FullName.Contains(keyword) ||
                                     u.Username.Contains(keyword) ||
                                     u.Email.Contains(keyword));
        if (role.HasValue) query = query.Where(u => u.Role == role.Value);
        if (isActive.HasValue) query = query.Where(u => u.IsActive == isActive.Value);
        return await query.OrderBy(u => u.FullName)
                          .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(string? keyword, UserRole? role,
        bool? isActive, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(u => u.FullName.Contains(keyword) ||
                                     u.Username.Contains(keyword) ||
                                     u.Email.Contains(keyword));
        if (role.HasValue) query = query.Where(u => u.Role == role.Value);
        if (isActive.HasValue) query = query.Where(u => u.IsActive == isActive.Value);
        return await query.CountAsync(ct);
    }
}
