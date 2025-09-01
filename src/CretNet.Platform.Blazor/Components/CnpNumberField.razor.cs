using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpNumberField<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    protected string _controlKey = Guid.NewGuid().ToString();
    
    [Parameter] public TNumber? NullableValue { get; set; }
    [Parameter] public TNumber Value { get => NullableValue ?? TNumber.Zero; set => NullableValue = value; }
    [Parameter] public EventCallback<TNumber?> NullableValueChanged { get; set; }
    [Parameter] public EventCallback<TNumber> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string LeadingSign { get; set; } = string.Empty;
    [Parameter] public string TrailingSign { get; set; } = string.Empty;
    
    protected string DisplayValue => NullableValue is null ? "" : $"{LeadingSign}{Value:N2}{TrailingSign}";

    protected async Task SetValue(string? displayValue)
    {
        TNumber? result = null;
        
        if (decimal.TryParse(displayValue, NumberStyles.Any, CultureInfo.CurrentCulture, out var parsedValue))
            result = TNumber.CreateChecked(parsedValue);

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(result ?? TNumber.Zero);
        if (NullableValueChanged.HasDelegate)
            await NullableValueChanged.InvokeAsync(result);
        if (!ValueChanged.HasDelegate && !NullableValueChanged.HasDelegate)
            NullableValue = result;

        _controlKey = Guid.NewGuid().ToString();
    }
}