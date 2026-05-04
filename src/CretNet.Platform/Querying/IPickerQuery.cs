namespace CretNet.Platform.Querying;

/// <summary>
/// Server-side query for a search-as-you-type picker. Sibling to
/// <see cref="IPagedQuery{TRow}"/> but without paging — pickers
/// return a small fixed-cap result list, not pages.
/// </summary>
public interface IPickerQuery<TItem>
{
    /// <summary>
    /// Free-text search term. <c>null</c> or whitespace means
    /// "no filter — return the first <see cref="Limit"/> items
    /// in the handler's default order".
    /// </summary>
    string? Search { get; }

    /// <summary>
    /// Hard cap on the number of items returned. Pickers never
    /// page; instead they show top-N matches and the handler
    /// truncates anything over <see cref="Limit"/>.
    /// </summary>
    int Limit { get; }
}
