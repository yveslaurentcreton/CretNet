using Fluxor;
using CretNet.Platform.Fluxor;
using CretNet.Platform.Fluxor.Generators;

namespace CretNet.Platform.Playground;

[CnpEntityAction(typeof(Customer), false, true)]
[CnpEntityState(typeof(CustomerState), nameof(CustomerState.Current), nameof(CustomerState.IsSaving), true)]
public partial class FetchCustomerAction
{
    [CnpActionLabel] public static string ActionLabel => "Fetch Customer";
    [CnpSuccessLabel] public static string SuccessLabel => "Customers have been fetched";
    
    [CnpInject] public ICustomerService CustomerService { get; set; } = default!;
    
    public Task<Customer?> Effect(IDispatcher dispatcher)
    {
        throw new NotImplementedException();
    }

    public CustomerState Reduce(CustomerState state, ICnpEntitySuccessAction<Customer?> action)
    {
        throw new NotImplementedException();
    }
}

[CnpAction]
[CnpState(typeof(CustomerState), true)]
public partial class SetCustomerAction()
{
    public CustomerState Reduce(CustomerState state)
    {
        throw new NotImplementedException();
    }
}

public class Customer
{
    public int Id { get; set; }
    public string? Number { get; set; }
}

public interface ICustomerService
{

}

public record CustomerState
{
    public IEnumerable<Customer>? Customers { get; init; }
    public Customer? Current { get; init; }
    public bool IsSaving { get; init; }
}

public abstract class Labels
{
    public const string Customer = "Customer";
    public const string Customers = "Customers";
}
