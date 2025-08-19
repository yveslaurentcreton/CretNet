using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSelectEnum<TEnum>
    where TEnum : struct, Enum
{
    [Parameter] public TEnum? Option { get; set; }
    [Parameter] public EventCallback<TEnum?> OptionChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public Func<TEnum?,string?>? OptionString { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Required { get; set; }

    public IEnumerable<TEnum?> Options => Required ? Enum.GetValues<TEnum>().Cast<TEnum?>() : [null, ..Enum.GetValues<TEnum>().Cast<TEnum?>()];
}