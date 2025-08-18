namespace CretNet.Platform.Blazor.Services.Countries;

public interface ICountryService
{
    IEnumerable<Country> GetCountries();
    Country GetCountry(string twoLetterIsoCode);
}