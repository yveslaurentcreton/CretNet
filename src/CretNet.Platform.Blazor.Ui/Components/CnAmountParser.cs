using System.Globalization;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Reads an amount the way people type one, whichever separators they
/// reach for.
/// </summary>
/// <remarks>
/// Accepts "1.234,56", "1234,56", "1234.56", "12.34" and plain integers.
/// When both separators occur the rightmost is the decimal one. A single
/// dot followed by exactly three digits reads as a thousands separator
/// ("1.500" is fifteen hundred, the Belgian habit); any other single dot
/// is a decimal point. Currency signs and spaces are ignored.
/// </remarks>
public static class CnAmountParser
{
    /// <summary>The parsed amount, or null for blank or unreadable input.</summary>
    public static decimal? Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var text = raw
            .Replace("€", string.Empty)
            .Replace("$", string.Empty)
            .Replace("£", string.Empty)
            .Replace(" ", string.Empty)
            .Replace(" ", string.Empty)
            .Trim();

        var lastComma = text.LastIndexOf(',');
        var lastDot = text.LastIndexOf('.');

        if (lastComma >= 0 && lastDot >= 0)
        {
            text = lastComma > lastDot
                ? text.Replace(".", string.Empty).Replace(',', '.')
                : text.Replace(",", string.Empty);
        }
        else if (lastComma >= 0)
        {
            text = text.Count(c => c == ',') == 1
                ? text.Replace(',', '.')
                : text.Replace(",", string.Empty);
        }
        else if (lastDot >= 0)
        {
            var digitsAfter = text.Length - lastDot - 1;
            if (text.Count(c => c == '.') > 1 || digitsAfter == 3)
                text = text.Replace(".", string.Empty);
        }

        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    /// <summary>The sign shown inside a currency field: "€" for EUR, "$" for
    /// USD, "£" for GBP, and the code itself for anything else.</summary>
    public static string SignFor(string? currency)
    {
        var code = currency?.Trim().ToUpperInvariant();
        return code switch
        {
            null or "" or "EUR" => "€",
            "USD" => "$",
            "GBP" => "£",
            _ => code,
        };
    }
}
