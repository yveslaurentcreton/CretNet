namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>Server-side page request emitted by <see cref="CnDataGrid{TItem}"/>.</summary>
public sealed record CnGridRequest(string? Search, int PageIndex, int PageSize, string? SortField, bool SortDescending);

/// <summary>One page of grid data.</summary>
public sealed record CnGridPage<TItem>(IReadOnlyList<TItem> Items, int TotalCount);

public enum CnGridAlign
{
    Left,
    Right,
    Center,
}
