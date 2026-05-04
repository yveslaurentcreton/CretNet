using BackedBySample.Models;
using BackedBySample.Services;
using CretNet.Platform.Fluxor.Generators;
using CretNet.Platform.Querying;
using Fluxor;

namespace BackedBySample.State.Actions;

[CnpEntityAction(typeof(PagedResult<SampleCustomer>))]
public partial class FetchSampleCustomersAction(SampleCustomerQuery Query)
{
    [CnpInject] public InMemoryCustomerService CustomerService { get; set; } = default!;

    public Task<PagedResult<SampleCustomer>> Effect(IDispatcher dispatcher) =>
        CustomerService.QueryAsync(Query);
}
