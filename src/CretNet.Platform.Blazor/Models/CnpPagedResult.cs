namespace CretNet.Platform.Blazor.Models;

public class CnpPagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
}
