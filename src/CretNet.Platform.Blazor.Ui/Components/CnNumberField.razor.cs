using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// A plain number: hours, a percentage, a count. Null is "nothing entered".
/// </summary>
/// <remarks>
/// Deliberately not a number input. Those bring spinners nobody asked for,
/// format by the browser's locale rather than the application's, and lose
/// a half-typed comma on re-render. A text field with
/// <see cref="CnAmountParser"/> behind it reads what people type and shows
/// it back in the current culture.
/// </remarks>
public partial class CnNumberField
{
    /// <summary>A suffix inside the field — "%", "h".</summary>
    [Parameter] public string? Unit { get; set; }

    /// <summary>Clamped on commit: a percentage stays between 0 and 100
    /// without a spinner to say so.</summary>
    [Parameter] public decimal? Min { get; set; }

    [Parameter] public decimal? Max { get; set; }

    /// <summary>Decimals shown. A value with more keeps them; the display
    /// rounds.</summary>
    [Parameter] public int MaxDecimals { get; set; } = 2;

    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    private string? UnitClass => string.IsNullOrEmpty(Unit) ? null : "cn-money-input--unit";

    private string Text =>
        Value is { } number
            ? number.ToString("#,##0." + new string('#', Math.Max(0, MaxDecimals)), CultureInfo.CurrentCulture)
            : string.Empty;

    private Task OnChangeAsync(ChangeEventArgs args)
    {
        var value = CnAmountParser.Parse(args.Value?.ToString());

        if (value is { } number)
        {
            if (Min is { } min && number < min)
                number = min;
            if (Max is { } max && number > max)
                number = max;
            value = number;
        }

        return SetValueAsync(value);
    }
}
