using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CretNet.Platform.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CretNet.Platform.Data;

public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : notnull
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DbContext _context;

    public Repository(IServiceProvider serviceProvider, DbContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
    }

    public Task<IPagedResult<TEntity>> GetAllAsync(ISpecification<TEntity> spec, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        return GetAllAsync(paging: null, spec: spec, asTracking: asTracking, cancellationToken: cancellationToken);
    }

    public async Task<IPagedResult<TEntity>> GetAllAsync(PagingOptions? paging = null,
        ISpecification<TEntity>? spec = null, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(spec, asTracking);

        if (!string.IsNullOrWhiteSpace(paging?.Search))
        {
            var searchSpec = _serviceProvider.GetService<IEntitySearchSpecification<TEntity>>();
            if (searchSpec is not null)
            {
                searchSpec.Configure(paging.Search);
                query = query.WithSpecification(searchSpec);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (paging is { PageIndex: not null, PageSize: not null })
        {
            query = query
                .Skip((paging.PageIndex.Value - 1) * paging.PageSize.Value)
                .Take(paging.PageSize.Value);
        }

        var items = await query.ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount,
            paging?.PageIndex ?? 1,
            paging?.PageSize ?? totalCount);
    }

    public async Task<TEntity?> GetAsync(TId id, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(asTracking: asTracking);

        return await query
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity?> GetAsync(ISpecification<TEntity> spec, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(spec, asTracking);

        return await query
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<TEntity?> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity == null)
            return null;

        _context.Set<TEntity>().Remove(entity);
        return entity;
    }

    public async Task<TEntity?> DeleteAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>()
            .AsQueryable()
            .WithSpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
            return null;

        _context.Set<TEntity>().Remove(entity);
        return entity;
    }

    private IQueryable<TEntity> BuildQuery(ISpecification<TEntity>? spec = null, bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (asTracking)
            query = query.AsTracking();

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);

        if (spec is not null)
            query = query.WithSpecification(spec);

        return query;
    }
}
