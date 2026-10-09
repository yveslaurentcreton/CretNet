namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>Server-side page request emitted by <see cref="CnDataGrid{TItem}"/>.</summary>
public sealed record CnGridRequest(string? Search, int PageIndex, int PageSize, string? SortField, bool SortDescending);

/// <summary>One page of grid data.</summary>
public sealed record CnGridPage<TItem>(IReadOnlyList<TItem> Items, int TotalCount);

/// <summary>
/// A provider for a list the host already holds (the lines of the document on
/// screen, a ledger it loaded once): the grid's own search and paging, applied
/// in memory, so a small embedded list gets the same toolbar as a server list.
/// </summary>
public static class CnGridPage
{
    /// <summary>
    /// Filters <paramref name="items"/> with <paramref name="matches"/> when the
    /// request carries a search (trimmed) and returns the requested page with the
    /// filtered count. Without <paramref name="matches"/> the search is ignored.
    /// </summary>
    public static CnGridPage<TItem> From<TItem>(
        IEnumerable<TItem> items,
        CnGridRequest request,
        Func<TItem, string, bool>? matches = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(request);

        var search = request.Search?.Trim();
        var filtered = (string.IsNullOrEmpty(search) || matches is null
                ? items
                : items.Where(item => matches(item, search)))
            .ToList();
        var pageSize = Math.Max(1, request.PageSize);
        var pageIndex = Math.Max(1, request.PageIndex);
        return new CnGridPage<TItem>(
            filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            filtered.Count);
    }

    /// <summary>True when any of <paramref name="texts"/> contains <paramref name="search"/>, ignoring case.</summary>
    public static bool Matches(string search, params string?[] texts) =>
        texts.Any(text => text?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true);
}

public enum CnGridAlign
{
    Left,
    Right,
    Center,
}
