using Fluxor;
using CretNet.Platform.Blazor.State;
using IsoNames;

namespace CretNet.Platform.Blazor.Services.Countries;

public class CountryService : ICountryService
{
    private readonly IState<CnpSiteState> _cnpSiteState;

    public CountryService(IState<CnpSiteState> cnpSiteState)
    {
        _cnpSiteState = cnpSiteState;
    }

    public IEnumerable<Country> GetCountries()
    {
        var countryTwoLetterIsoCodes = GetTwoLetterCountryIsoCodes();
        var cultureInfo = _cnpSiteState.Value.CurrentCulture;
        var countries = countryTwoLetterIsoCodes.Select(twoLetterIsoCode =>
        {
            return new Country()
            {
                TwoLetterIsoCode = twoLetterIsoCode,
                Name = CountryNames.GetName(cultureInfo, twoLetterIsoCode)
            };
        })
            .Where(country => !string.IsNullOrEmpty(country.Name))
            .OrderBy(country => country.Name);

        return countries;
    }
    
    public Country GetCountry(string twoLetterIsoCode)
    {
        var cultureInfo = _cnpSiteState.Value.CurrentCulture;
        return new Country()
        {
            TwoLetterIsoCode = twoLetterIsoCode,
            Name = CountryNames.GetName(cultureInfo, twoLetterIsoCode)
        };
    }

    private IEnumerable<string> GetTwoLetterCountryIsoCodes()
    {
        return ISO3166.Country.List.Select(country => country.TwoLetterCode);
    }
}
