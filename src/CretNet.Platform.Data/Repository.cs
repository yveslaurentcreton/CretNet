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
        var query = _context.Set<TEntity>().AsQueryable();
        
        if (asTracking)
            query = query.AsTracking();

        var baseSpec = _serviceProvider.GetService<IEntityDefaultSpecification<TEntity>>();
        if (baseSpec is not null)
            query = query.WithSpecification(baseSpec);

        if (spec is not null)
            query = query.WithSpecification(spec);

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
}