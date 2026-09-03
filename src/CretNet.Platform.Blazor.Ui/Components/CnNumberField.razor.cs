using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// A plain number: hours, a percentage, a count. Null is "nothing entered".
/// </summary>
public partial class CnNumberField
{
    /// <summary>A suffix inside the field — "%", "h".</summary>
    [Parameter] public string? Unit { get; set; }

    /// <summary>The browser's step, also what the spinners move by. "any"
    /// accepts every decimal.</summary>
    [Parameter] public string Step { get; set; } = "any";

    [Parameter] public decimal? Min { get; set; }

    [Parameter] public decimal? Max { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    private string? UnitClass => string.IsNullOrEmpty(Unit) ? null : "cn-money-input--unit";

    /// <summary>Invariant, because that is what a number input reads; and
    /// without the trailing zeros a database scale leaves behind — 65,
    /// not 65.000.</summary>
    private string Text => Value?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty;

    private Task OnChangeAsync(ChangeEventArgs args)
    {
        var raw = args.Value?.ToString();
        var value = decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : (decimal?)null;

        return SetValueAsync(value);
    }
}
