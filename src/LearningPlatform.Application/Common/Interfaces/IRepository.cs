using System.Linq.Expressions;
using LearningPlatform.Domain.Common;
using LearningPlatform.Shared.Pagination;

namespace LearningPlatform.Application.Common.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<PaginatedList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change-tracked lookup (unlike FindAsync/AsQueryable, which are AsNoTracking). Use when
    /// the same entity may be fetched-and-mutated more than once within one request — EF's
    /// identity map then returns the SAME tracked instance on the second call instead of a
    /// detached duplicate, avoiding a "same key already tracked" collision on Update().
    /// </summary>
    Task<T?> GetTrackedAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Escape hatch for read-only queries that need Include/projection composed by the
    /// caller (e.g. list endpoints), so they don't pay for N+1 loads through the
    /// simpler Get/Find methods above.
    /// </summary>
    IQueryable<T> AsQueryable();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
