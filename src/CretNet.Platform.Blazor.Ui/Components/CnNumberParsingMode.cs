namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>How a numeric input resolves decimal and grouping separators.</summary>
public enum CnNumberParsingMode
{
    /// <summary>Accept either decimal separator; a lone dot with three trailing
    /// digits is a thousands separator. Preserves the currency-entry convention.</summary>
    Flexible,

    /// <summary>A single comma or dot is decimal, even before three digits.
    /// When both appear, the rightmost is decimal and the other groups thousands.</summary>
    Decimal,

    /// <summary>Use the current culture's decimal and grouping separators.</summary>
    Culture,
}
