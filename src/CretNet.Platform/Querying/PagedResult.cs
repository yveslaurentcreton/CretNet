namespace CretNet.Platform.Querying;

/// <summary>
/// The result of an <see cref="IPagedQuery{TRow}"/>: a single page of
/// projected rows together with the total row count that satisfied the
/// query (irrespective of paging).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TotalCount"/> reflects the rows the server-side handler
/// would return if paging were not applied. It is the source of truth
/// for the paginator UI; clients must not recompute it from
/// <see cref="Items"/>.
/// </para>
/// <para>
/// <see cref="Items"/> is exposed as <see cref="IReadOnlyList{TRow}"/>
/// so consumers can index into it and count without enumerating twice.
/// </para>
/// </remarks>
public sealed record PagedResult<TRow>
{
    public required IReadOnlyList<TRow> Items { get; init; }

    public required int TotalCount { get; init; }

    public required int PageIndex { get; init; }

    public required int PageSize { get; init; }

    public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;

    public static PagedResult<TRow> Empty(int pageIndex = 1, int pageSize = 20) => new()
    {
        Items = Array.Empty<TRow>(),
        TotalCount = 0,
        PageIndex = pageIndex,
        PageSize = pageSize,
    };
}
