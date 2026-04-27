namespace CretNet.Platform.Querying;

/// <summary>
/// A query that returns a <see cref="PagedResult{TRow}"/> of <typeparamref name="TRow"/>
/// projections. The interface exposes the standard envelope fields
/// (paging, search, sort) that every paged query supports, so generic
/// infrastructure (data sources, debounced search, paging guards) can
/// operate against any concrete query type.
/// </summary>
/// <remarks>
/// <para>
/// Concrete queries are typically records that add their own filter
/// fields and also implement the consumer's mediator contract
/// (e.g. <c>MediatR.IRequest&lt;PagedResult&lt;TRow&gt;&gt;</c>) — that
/// extra contract is intentionally not declared here so this assembly
/// stays free of mediator dependencies.
/// </para>
/// <para>
/// The properties are read-only by design. Mutation flows through
/// <c>with</c>-expressions on the concrete record, driven by the
/// per-screen <c>QueryState&lt;TQuery&gt;</c> wrapper in the Blazor
/// layer.
/// </para>
/// </remarks>
public interface IPagedQuery<TRow>
{
    /// <summary>
    /// One-based page index. The first page is <c>1</c>.
    /// </summary>
    int PageIndex { get; }

    /// <summary>
    /// Page size. Must be positive.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Optional free-text search term. <c>null</c> or whitespace means
    /// "do not filter by search".
    /// </summary>
    string? Search { get; }

    /// <summary>
    /// Optional sort spec. <c>null</c> means "use the handler's default
    /// sort order".
    /// </summary>
    SortSpec? Sort { get; }
}
