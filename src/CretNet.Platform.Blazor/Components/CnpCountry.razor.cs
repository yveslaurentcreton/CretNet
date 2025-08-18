using CretNet.Platform.Blazor.Services.Countries;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpCountry
{
    [Parameter, EditorRequired] public string CountryCode { get; set; } = default!;
    
    [Inject] public ICountryService CountryService { get; set; } = default!;
    
    private Country Country => CountryService.GetCountry(CountryCode);
}