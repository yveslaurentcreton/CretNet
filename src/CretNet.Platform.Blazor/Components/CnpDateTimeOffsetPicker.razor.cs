using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDateTimeOffsetPicker
{
    [Parameter] public DateTimeOffset? NullableValue { get; set; }
    [Parameter] public DateTimeOffset? Value { get => NullableValue ?? default; set => NullableValue = value; }
    [Parameter] public EventCallback<DateTimeOffset?> NullableValueChanged { get; set; }
    [Parameter] public EventCallback<DateTimeOffset> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    
    [Inject] public ITimeService TimeService { get; set; } = default!;

    private DateTime? DisplayValue { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        var local = TimeService.ConvertToLocalDateTimeOffset(NullableValue);
        DisplayValue = local?.DateTime;
    }

    protected async Task SetValue(DateTime? displayValue)
    {
        DisplayValue = displayValue;
        
        var result = TimeService.ConvertToLocalDateTimeOffset(displayValue);
        
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(result ?? default);
        if (NullableValueChanged.HasDelegate)
            await NullableValueChanged.InvokeAsync(result);
        if (!ValueChanged.HasDelegate && !NullableValueChanged.HasDelegate)
            NullableValue = result;
    }
}