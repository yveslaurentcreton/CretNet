using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDatePicker<TDate> : CnpComponent
    where TDate : struct
{
    protected string _controlKey = Guid.NewGuid().ToString();

    [Parameter] public TDate? NullableValue { get; set; }
    [Parameter] public TDate Value { get => NullableValue ?? default; set => NullableValue = value; }
    [Parameter] public EventCallback<TDate?> NullableValueChanged { get; set; }
    [Parameter] public EventCallback<TDate> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var type = typeof(TDate);
        if (type != typeof(DateTime) && type != typeof(DateTimeOffset))
        {
            throw new InvalidOperationException($"Type {type.Name} is not supported by CnpDatePicker. Use DateTime or DateTimeOffset.");
        }

        bool isValueBound = ValueChanged.HasDelegate;
        bool isNullableValueBound = NullableValueChanged.HasDelegate;

        if (isValueBound && isNullableValueBound)
        {
            throw new InvalidOperationException("CnpDatePicker cannot be bound to both Value and NullableValue simultaneously.");
        }

        if (!isValueBound && !isNullableValueBound)
        {
            throw new InvalidOperationException("CnpDatePicker must be bound to either Value or NullableValue.");
        }
    }

    protected DateTime? DisplayValue
    {
        get
        {
            if (!NullableValue.HasValue)
              return null;
              
            TDate actualValue = NullableValue.Value;

            if (typeof(TDate) == typeof(DateTime))
                return (DateTime)(object)actualValue;
            if (typeof(TDate) == typeof(DateTimeOffset))
                return ((DateTimeOffset)(object)actualValue).DateTime;
            throw new InvalidOperationException($"Unsupported type for DisplayValue: {typeof(TDate)}");
        }
    }

    protected async Task SetValue(DateTime? value)
    {
        TDate? result = null;

        if (value.HasValue)
        {
            if (typeof(TDate) == typeof(DateTime))
            {
                result = (TDate)(object)value.Value;
            }
            else if (typeof(TDate) == typeof(DateTimeOffset))
            {
                var offset = TimeZoneInfo.Local.GetUtcOffset(value.Value);
                result = (TDate)(object)new DateTimeOffset(value.Value, offset);
            }
        }
        
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(result ?? default);
        if (NullableValueChanged.HasDelegate)
            await NullableValueChanged.InvokeAsync(result);
        if (!ValueChanged.HasDelegate && !NullableValueChanged.HasDelegate)
            NullableValue = result;

        _controlKey = Guid.NewGuid().ToString();
    }
}
