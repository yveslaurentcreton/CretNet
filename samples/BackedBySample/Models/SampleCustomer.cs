using CretNet.Platform.Data;

namespace BackedBySample.Models;

public class SampleCustomer : IIdentity<Guid>
{
    public Guid Id { get; set; }
    public required string Number { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
}
