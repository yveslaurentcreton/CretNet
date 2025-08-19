using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpNullableDecimalField
{
    protected string _controlKey = Guid.NewGuid().ToString();
    
    [Parameter] public decimal? Value { get; set; }
    [Parameter] public EventCallback<decimal?> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    
    protected string DisplayValue => $"{Value:N2}";

    protected async Task SetValue(string? displayValue)
    {
        decimal result = 0;
        var filteredValue = new string(displayValue?.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        if (decimal.TryParse(filteredValue, System.Globalization.NumberStyles.Currency,
                System.Globalization.CultureInfo.CurrentCulture, out var parsedValue))
        {
            result = parsedValue;
        }

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(result);
        else
            Value = result;

        _controlKey = Guid.NewGuid().ToString();
    }
}