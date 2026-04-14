using Ardalis.Specification;

namespace CretNet.Platform.Data.Abstractions;

public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId> where TId : notnull
{
    Task<IPagedResult<TEntity>> GetAllAsync(PagingOptions? paging = null, ISpecification<TEntity>? spec = null, bool asTracking = false, CancellationToken cancellationToken = default);
    Task<IPagedResult<TEntity>> GetAllAsync(ISpecification<TEntity> spec, bool asTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(TId id, bool asTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(ISpecification<TEntity> spec, bool asTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity?> DeleteAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
}
