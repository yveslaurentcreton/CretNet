namespace CretNet.Platform.Data.Abstractions;

public interface IPagedResult<T>
{
    IEnumerable<T> Items { get; }
    int TotalCount { get; }
    int PageIndex { get; }
    int PageSize { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}
