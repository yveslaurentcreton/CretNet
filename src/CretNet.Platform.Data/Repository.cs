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

    public async Task<IEnumerable<TEntity>> GetAll(ISpecification<TEntity>? spec = null, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(spec, asTracking);

        var entities = await query
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<TEntity?> GetAsync(TId id, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (asTracking)
            query = query.AsTracking();

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);

        var entity = await query
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

        return entity;
    }
    
    public async Task<TEntity?> GetAsync(ISpecification<TEntity> spec, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (asTracking)
            query = query.AsTracking();

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);
        
        var entity = await query
            .WithSpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
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
    
    public async Task<IPagedResult<TEntity>> GetPagedAsync(int pageIndex, int pageSize,
        ISpecification<TEntity>? spec = null, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(spec, asTracking);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<IEnumerable<TEntity>> Search(string searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        var searchSpec = _serviceProvider.GetService<IEntitySearchSpecification<TEntity>>();
        if (searchSpec is null)
            return [];
        searchSpec.Configure(searchTerm);
        query = query.WithSpecification(searchSpec);

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);

        var entities = await query
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<IPagedResult<TEntity>> SearchPagedAsync(string searchTerm, int pageIndex, int pageSize,
        ISpecification<TEntity>? spec = null, bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (asTracking)
            query = query.AsTracking();

        var searchSpec = _serviceProvider.GetService<IEntitySearchSpecification<TEntity>>();
        if (searchSpec is null)
            return PagedResult<TEntity>.Empty(pageIndex, pageSize);
        searchSpec.Configure(searchTerm);
        query = query.WithSpecification(searchSpec);

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);

        if (spec is not null)
            query = query.WithSpecification(spec);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageIndex, pageSize);
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