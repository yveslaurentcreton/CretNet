using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// An amount of money. Null is "nothing entered", which is not the same as
/// zero: a project without a fee is not a project for free.
/// </summary>
public partial class CnCurrencyField
{
    /// <summary>ISO 4217 code. EUR, USD and GBP show their sign; anything
    /// else shows the code.</summary>
    [Parameter] public string Currency { get; set; } = "EUR";

    /// <summary>A suffix inside the field — "/h" for a rate, say.</summary>
    [Parameter] public string? Unit { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    [Parameter] public int Decimals { get; set; } = 2;

    private string Sign => CnAmountParser.SignFor(Currency);

    private string? UnitClass => string.IsNullOrEmpty(Unit) ? null : "cn-money-input--unit";

    private string Text =>
        Value is { } amount
            ? amount.ToString($"N{Decimals}", CultureInfo.CurrentCulture)
            : string.Empty;

    private Task OnChangeAsync(ChangeEventArgs args) =>
        SetValueAsync(CnAmountParser.Parse(args.Value?.ToString()));
}
