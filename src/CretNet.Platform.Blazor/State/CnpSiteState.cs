using System.Globalization;

namespace CretNet.Platform.Blazor.State;

public record CnpSiteState(
    CultureInfo? CurrentCulture)
{
}