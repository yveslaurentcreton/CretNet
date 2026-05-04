using BackedBySample.Models;
using CretNet.Platform.Querying;

namespace BackedBySample.Services;

/// <summary>
/// Canonical BackedBy sample data source — runs entirely in-process,
/// no network, no database. Demonstrates the contract a real fetch
/// action must satisfy: take the typed query, apply filter/sort/page,
/// return a <see cref="PagedResult{T}"/>.
/// </summary>
public sealed class InMemoryCustomerService
{
    private readonly List<SampleCustomer> _all;

    public InMemoryCustomerService()
    {
        _all = Enumerable.Range(1, 100)
            .Select(i => new SampleCustomer
            {
                Id = Guid.NewGuid(),
                Number = $"C{i:D5}",
                Name = $"Customer {i}",
                City = i % 2 == 0 ? "Brussels" : "Antwerp",
            })
            .ToList();
    }

    public Task<PagedResult<SampleCustomer>> QueryAsync(SampleCustomerQuery query)
    {
        IEnumerable<SampleCustomer> q = _all;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search;
            q = q.Where(c =>
                c.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Number.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.City.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Sort is not null)
        {
            q = (query.Sort.Field, query.Sort.Direction) switch
            {
                ("Name", SortDirection.Ascending) => q.OrderBy(c => c.Name),
                ("Name", SortDirection.Descending) => q.OrderByDescending(c => c.Name),
                ("Number", SortDirection.Ascending) => q.OrderBy(c => c.Number),
                ("Number", SortDirection.Descending) => q.OrderByDescending(c => c.Number),
                ("City", SortDirection.Ascending) => q.OrderBy(c => c.City),
                ("City", SortDirection.Descending) => q.OrderByDescending(c => c.City),
                _ => q.OrderBy(c => c.Number),
            };
        }
        else
        {
            q = q.OrderBy(c => c.Number);
        }

        var items = q.ToList();
        var page = items
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return Task.FromResult(new PagedResult<SampleCustomer>
        {
            Items = page,
            TotalCount = items.Count,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
        });
    }
}
