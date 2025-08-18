using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDateTimeOffset
{
    [Parameter] public DateTimeOffset? Value { get; set; }
    [Parameter] public string? Format { get; set; }
    
    [Inject] public ITimeService TimeService { get; set; } = default!;
    
    private DateTimeOffset? LocalTime { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        if (Value.HasValue)
        {
            LocalTime = TimeService.ConvertToLocalDateTimeOffset(Value.Value);
        }
        else
        {
            LocalTime = null;
        }
    }
}