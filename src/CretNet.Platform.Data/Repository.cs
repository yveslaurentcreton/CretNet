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

    public Task<IPagedResult<TEntity>> GetAllAsync(ISpecification<TEntity> specification, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        return GetAllAsync(paging: null, spec: specification, asTracking: asTracking, cancellationToken: cancellationToken);
    }

    public async Task<IPagedResult<TEntity>> GetAllAsync(PagingOptions? paging = null,
        ISpecification<TEntity>? spec = null, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        // Reject obviously invalid paging values before hitting the database
        if (paging is not null)
        {
            if (paging.PageIndex is null != paging.PageSize is null)
                throw new ArgumentException(
                    "PagingOptions must provide both PageIndex and PageSize, or neither.",
                    nameof(paging));
            if (paging.PageIndex is int pageIndex && pageIndex < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(paging), pageIndex, "PagingOptions.PageIndex must be 1 or greater.");
            if (paging.PageSize is int pageSize && pageSize < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(paging), pageSize, "PagingOptions.PageSize must be 1 or greater.");
        }

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

        var isPaged = paging is { PageIndex: not null, PageSize: not null };

        int totalCount;
        List<TEntity> items;

        if (isPaged)
        {
            // Paged reads need a separate count query so the paginator sees the full size
            totalCount = await query.CountAsync(cancellationToken);

            items = await query
                .Skip((paging!.PageIndex!.Value - 1) * paging.PageSize!.Value)
                .Take(paging.PageSize!.Value)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // Unpaged reads can derive the total count from the fetched list — skip the extra CountAsync
            items = await query.ToListAsync(cancellationToken);
            totalCount = items.Count;
        }

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
