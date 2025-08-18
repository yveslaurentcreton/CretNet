namespace CretNet.Platform.Blazor.Services.Countries;

public record Country
{
    public required string TwoLetterIsoCode { get; init; }
    public required string? Name { get; init; }
}
