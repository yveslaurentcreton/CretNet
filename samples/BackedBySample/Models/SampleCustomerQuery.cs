using CretNet.Platform.Querying;

namespace BackedBySample.Models;

public sealed record SampleCustomerQuery(
    string? Search = null,
    int PageIndex = 1,
    int PageSize = 10,
    SortSpec? Sort = null
) : IPagedQuery<SampleCustomer>;
