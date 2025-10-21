using System;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IBaseRepository<T>
{
    /// <summary>
    /// Creates an new entity in the repository
    /// </summary>
    /// <returns>The created entity</returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the repository
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, string includes = "");

    /// <summary>
    /// Deletes an entity from the repository
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    Task<T?> DeleteAsync(Guid id, CancellationToken cancellationToken = default, string includes = "");

    /// <summary>
    /// Gets an entity that satisfies the expression
    /// </summary>
    /// <param name="value">The expression to be used on the query</param>
    /// <returns>The enity if found, null otherwise</returns>
    Task<T?> GetOneByExpression(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable object for the repository
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IQueryable<T> Query(CancellationToken cancellationToken = default);
}
