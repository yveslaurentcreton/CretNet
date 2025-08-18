using CretNet.Platform.Blazor.Services.Countries;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpCountrySelect
{
    [Parameter] public string? CountryCode { get; set; }
    [Parameter] public EventCallback<string?> CountryCodeChanged { get; set; }
    [Parameter] public bool Required { get; set; }
    
    [Inject] public ICountryService CountryService { get; set; } = default!;
    
    public IEnumerable<Country> Countries { get; set; } = [];
    private Country? SelectedCountry => Countries.FirstOrDefault(country => country.TwoLetterIsoCode == CountryCode);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Countries = CountryService.GetCountries();
    }

    private async Task SetSelectedOption(Country? country)
    {
        await CountryCodeChanged.InvokeAsync(country?.TwoLetterIsoCode);
    }
}