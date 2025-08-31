using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpPercentageField<TNumber>
    where TNumber : struct, INumber<TNumber>
{
    [Parameter] public TNumber? NullableValue { get; set; }
    [Parameter] public TNumber Value { get => NullableValue ?? TNumber.Zero; set => NullableValue = value; }
    [Parameter] public EventCallback<TNumber?> NullableValueChanged { get; set; }
    [Parameter] public EventCallback<TNumber> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
}