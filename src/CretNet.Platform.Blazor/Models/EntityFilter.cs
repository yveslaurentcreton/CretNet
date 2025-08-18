using ReactiveUI;
using System.ComponentModel;

namespace CretNet.Platform.Blazor.Models;

public class EntityFilter<TEntity> : ReactiveObject, IEntityFilter
{
    public required string Category { get; init; }
    public required int Sequence { get; init; }
    public required string Key { get; init; }
    public required bool Enabled { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public required bool DefaultEnabled { get; init; }
    public required Func<IQueryable<TEntity>, IQueryable<TEntity>> Filter { get; init; }

    public IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query)
    {
        return Enabled ? Filter(query) : query;
    }
    
    // Non-generic implementation for the interface
    public IQueryable ApplyFilter(IQueryable query)
    {
        if (!Enabled) return query;
        
        if (query is IQueryable<TEntity> typedQuery)
        {
            return ApplyFilter(typedQuery);
        }
        
        // If the query is not of the expected type, return it unchanged
        return query;
    }
}

public interface IEntityFilter : INotifyPropertyChanged
{
    string Category { get; }
    int Sequence { get; }
    string Key { get; }
    bool Enabled { get; set; }
    
    IQueryable ApplyFilter(IQueryable query);
}

public enum FilterCombineMode
{
    And,    // All filters must match (intersection)
    Or,     // Any filter can match (union)
    CategoryOr // Filters within same category are OR'd, different categories are AND'd
}

public static class FilterExtensions
{
    public static IQueryable<TEntity> ApplyFilters<TEntity>(
        this IQueryable<TEntity> query, 
        IEnumerable<EntityFilter<TEntity>> filters, 
        FilterCombineMode mode = FilterCombineMode.And)
    {
        var enabledFilters = filters.Where(f => f.Enabled).ToList();
        if (!enabledFilters.Any()) return query;

        return mode switch
        {
            FilterCombineMode.And => ApplyAndFilters(query, enabledFilters),
            FilterCombineMode.Or => ApplyOrFilters(query, enabledFilters),
            FilterCombineMode.CategoryOr => ApplyCategoryOrFilters(query, enabledFilters),
            _ => query
        };
    }
    
    private static IQueryable<TEntity> ApplyAndFilters<TEntity>(
        IQueryable<TEntity> query, 
        List<EntityFilter<TEntity>> filters)
    {
        return filters.Aggregate(query, (current, filter) => filter.ApplyFilter(current));
    }
    
    private static IQueryable<TEntity> ApplyOrFilters<TEntity>(
        IQueryable<TEntity> query, 
        List<EntityFilter<TEntity>> filters)
    {
        if (filters.Count == 1)
            return filters[0].ApplyFilter(query);
            
        // For OR logic, we need to apply each filter to the original query and union results
        var results = filters.Select(filter => filter.ApplyFilter(query));
        return results.Aggregate((current, next) => current.Union(next));
    }
    
    private static IQueryable<TEntity> ApplyCategoryOrFilters<TEntity>(
        IQueryable<TEntity> query, 
        List<EntityFilter<TEntity>> filters)
    {
        var categorizedFilters = filters.GroupBy(f => f.Category);
        
        foreach (var categoryGroup in categorizedFilters)
        {
            var categoryFilters = categoryGroup.ToList();
            if (categoryFilters.Count == 1)
            {
                query = categoryFilters[0].ApplyFilter(query);
            }
            else
            {
                // OR within category
                var categoryResults = categoryFilters.Select(filter => filter.ApplyFilter(query));
                var unionResult = categoryResults.Aggregate((current, next) => current.Union(next));
                query = unionResult;
            }
        }
        
        return query;
    }
}