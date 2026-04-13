using CretNet.Platform.Data.Abstractions;

namespace CretNet.Platform.Data;

public class PagedResult<T> : IPagedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalCount { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static PagedResult<T> Empty(int pageIndex = 1, int pageSize = 20)
        => new([], 0, pageIndex, pageSize);
}
