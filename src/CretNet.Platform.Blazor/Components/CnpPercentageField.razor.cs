using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpPercentageField<TNumber>
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

    private string DisplayValue => NullableValue is null ? "" : $"{NullableValue:N2} %";

    private async Task SetValue(string? displayValue)
    {
        TNumber? result = null;
        var filteredValue = new string(displayValue?.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        if (decimal.TryParse(filteredValue, System.Globalization.NumberStyles.Currency,
                System.Globalization.CultureInfo.CurrentCulture, out var parsedValue))
        {
            result = TNumber.CreateChecked(parsedValue);
        }

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(result ?? TNumber.Zero);
        if (NullableValueChanged.HasDelegate)
            await NullableValueChanged.InvokeAsync(result);
        if (!ValueChanged.HasDelegate && !NullableValueChanged.HasDelegate)
            NullableValue = result;

        _controlKey = Guid.NewGuid().ToString();
    }
}