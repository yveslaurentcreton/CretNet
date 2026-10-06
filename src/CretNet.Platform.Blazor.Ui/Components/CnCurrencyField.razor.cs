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
    /// <summary>The dense variant for a dialog's header row and the cells of
    /// its line list: the label before the field instead of above it, a 26px
    /// input with 13px text. Combines with Subtle; off, the field is unchanged.</summary>
    [Parameter] public bool Compact { get; set; }

    private string? CompactClass => Compact ? "cn-field--compact" : null;


    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    [Parameter] public int Decimals { get; set; } = 2;

    /// <summary>How ambiguous separators are interpreted; independent of currency and display precision.</summary>
    [Parameter] public CnNumberParsingMode ParsingMode { get; set; } = CnNumberParsingMode.Flexible;

    private string Sign => CnAmountParser.SignFor(Currency);

    private string? UnitClass => string.IsNullOrEmpty(Unit) ? null : "cn-money-input--unit";

    /// <summary>The unit's length in characters, so the input keeps room for
    /// the whole unit before its digits: "dagen" needs more than "%".</summary>
    private string? UnitStyle => string.IsNullOrEmpty(Unit)
        ? null
        : string.Create(CultureInfo.InvariantCulture, $"--cn-unit-chars: {Unit.Length}");

    private string Text =>
        Value is { } amount
            ? amount.ToString($"N{Decimals}", CultureInfo.CurrentCulture)
            : string.Empty;

    private Task OnChangeAsync(ChangeEventArgs args) =>
        SetValueAsync(CnAmountParser.Parse(args.Value?.ToString(), ParsingMode));
}
