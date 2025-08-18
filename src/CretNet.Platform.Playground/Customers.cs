using Fluxor;
using CretNet.Platform.Fluxor.Generators;

namespace CretNet.Platform.Playground;

[CnpEntityAction(typeof(IEnumerable<Customer>))]
[CnpEntityState(typeof(CustomerState), nameof(CustomerState.Customers), null)]
public partial class FetchCustomersAction
{
    [CnpActionLabel] public static string ActionLabel => "Fetch Customers";
    [CnpSuccessLabel] public static string SuccessLabel => "Customers have been fetched";
    [CnpFailureLabel] public static string FailureLabel => "Customers have been fetched";
    
    [CnpInject] public IEnumerable<Customer> CustomerService { get; set; } = default!;
    
    public Task<IEnumerable<Customer>> Effect(IDispatcher dispatcher)
    {
        throw new NotImplementedException();
    }
}
