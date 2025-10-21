using System.Globalization;
using System.Linq.Expressions;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DefaultContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the repository
    /// </summary>
    /// <param name="context">The database context</param>
    public BaseRepository(DefaultContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<TEntity?> DeleteAsync(Guid id, CancellationToken cancellationToken = default, string include = "")
    {
        var entity = await GetByIdAsync(id, cancellationToken, include);
        if (entity == null)
            return null;

        entity.MarkAsDeleted();
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, string include = "")
    {
        var query = _dbSet.Where(e => e.Id == id && !e.IsDeleted);
        if (!string.IsNullOrEmpty(include))
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> GetOneByExpression(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public IQueryable<TEntity> Query(CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking();
    }
}
