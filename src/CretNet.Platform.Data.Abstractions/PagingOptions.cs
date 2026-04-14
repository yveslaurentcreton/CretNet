namespace CretNet.Platform.Data.Abstractions;

public class PagingOptions
{
    public int? PageIndex { get; init; }
    public int? PageSize { get; init; }
    public string? Search { get; init; }
}
