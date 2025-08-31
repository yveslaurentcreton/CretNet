using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpCurrencyField
{
    [Parameter] public decimal Value { get; set; }
    [Parameter] public EventCallback<decimal> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
}