using BackedBySample.Models;
using BackedBySample.Pages;
using BackedBySample.State.Actions;
using CretNet.Platform.Blazor.Services;

namespace BackedBySample.Services;

public class SampleCustomersDefinition : EntityDefinition<SampleCustomer, Guid>
{
    public SampleCustomersDefinition()
    {
        Entity()
            .WithLabel("Sample Customer")
            .WithPluralLabel("Sample Customers")
            .WithIdentifier(c => c.Number)
            .WithDisplayName(c => c.Name)
            .WithDataGrid<SampleCustomersGrid>()
            .BackedBy<SampleCustomerQuery, FetchSampleCustomersAction>(query => new FetchSampleCustomersAction(query));
    }
}
