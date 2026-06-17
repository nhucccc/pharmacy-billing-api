using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;

namespace PharmacyBilling.Infrastructure.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly PharmacyDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(PharmacyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.Where(predicate).ToListAsync(ct);

    public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public virtual void Update(T entity)
        => _dbSet.Update(entity);

    public virtual void Remove(T entity)
        => _dbSet.Remove(entity);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate == null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AnyAsync(predicate, ct);
}
