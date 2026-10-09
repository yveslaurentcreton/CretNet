using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// A quantity and its unit in one field: "15 minuten", "2,5 uur", "3 stuks".
/// <see cref="CnInputBase{TValue}.Value"/> is the quantity (null is "nothing
/// entered", not zero); <see cref="Unit"/> is the unit, chosen from
/// <see cref="Units"/>.
/// </summary>
/// <remarks>
/// The unit type is the host's own: an enum, a record, a
/// <c>readonly record struct</c>. A struct's default value counts as "no unit";
/// use a nullable struct (<c>TUnit="Unit?"</c>) when the default is a real unit
/// that may be missing from <see cref="Units"/>. A unit that is set but not in
/// <see cref="Units"/> is still offered, so a value never silently disappears.
/// </remarks>
public partial class CnQuantityField<TUnit>
{
    /// <summary>The unit, compared with the default equality of
    /// <typeparamref name="TUnit"/> (records compare by value).</summary>
    [Parameter] public TUnit? Unit { get; set; }

    [Parameter] public EventCallback<TUnit?> UnitChanged { get; set; }

    /// <summary>The units to choose from, in display order.</summary>
    [Parameter, EditorRequired] public IEnumerable<TUnit> Units { get; set; } = [];

    /// <summary>The text of a unit for the current quantity, so a host can say
    /// "1 minuut" but "15 minuten". Defaults to the unit's ToString().</summary>
    [Parameter] public Func<TUnit, decimal?, string> UnitText { get; set; } = (unit, _) => unit?.ToString() ?? string.Empty;

    /// <summary>Locks only the unit; the quantity stays editable.</summary>
    [Parameter] public bool UnitDisabled { get; set; }

    /// <summary>Decimals shown. A value with more keeps them; the display
    /// rounds.</summary>
    [Parameter] public int MaxDecimals { get; set; } = 4;

    /// <summary>How ambiguous separators are interpreted. A quantity reads a
    /// single comma or dot as decimal by default (see the UI contract).</summary>
    [Parameter] public CnNumberParsingMode ParsingMode { get; set; } = CnNumberParsingMode.Decimal;

    /// <summary>Clamped on commit; <c>Min="0"</c> keeps a quantity positive.</summary>
    [Parameter] public decimal? Min { get; set; }

    [Parameter] public decimal? Max { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    /// <summary>The number input's id, for a host label elsewhere.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>The number's accessible name when there is no <c>Label</c>.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    /// <summary>The unit dropdown's accessible name. Defaults to "Unit".</summary>
    [Parameter] public string UnitAriaLabel { get => field ?? CnLabels.Unit; set; } = null!;
    #pragma warning restore BL0007

    /// <summary>The dense variant for a dialog's header row and the cells of
    /// its line list: the label before the field instead of above it, a 26px
    /// input with 13px text. Combines with Subtle; off, the field is unchanged.</summary>
    [Parameter] public bool Compact { get; set; }

    private string? CompactClass => Compact ? "cn-field--compact" : null;

    private string? FrameStateClass =>
        Disabled ? "cn-quantity-frame--disabled" : ReadOnly ? "cn-quantity-frame--readonly" : null;

    private List<TUnit> _options = [];

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        BuildOptions();
    }

    private bool HasUnit => Unit is not null && !EqualityComparer<TUnit?>.Default.Equals(Unit, default);

    private void BuildOptions()
    {
        _options = Units.ToList();
        if (HasUnit && !_options.Contains(Unit!))
            _options.Add(Unit!);
    }

    private int SelectedIndex => _options.FindIndex(option => EqualityComparer<TUnit?>.Default.Equals(option, Unit));

    private string Text => CnNumberField.Format(Value, ParsingMode, MaxDecimals);

    private Task OnQuantityChangeAsync(ChangeEventArgs args) =>
        SetValueAsync(CnNumberField.Clamp(CnAmountParser.Parse(args.Value?.ToString(), ParsingMode), Min, Max));

    private async Task OnUnitChangeAsync(ChangeEventArgs args)
    {
        var index = int.TryParse(args.Value?.ToString(), out var parsed) ? parsed : -1;
        Unit = index >= 0 && index < _options.Count ? _options[index] : default;
        await UnitChanged.InvokeAsync(Unit);
        BuildOptions();
    }
}
