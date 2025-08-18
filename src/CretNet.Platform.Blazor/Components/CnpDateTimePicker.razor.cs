using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDateTimePicker : FluentInputBase<DateTime?>
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("eval", $"setTimeout(() => {{ const input = document.getElementById('{Id}').shadowRoot.querySelector('input'); if(input) input.step = '1'; }}, 0)");
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    
    /// <summary />
    protected override string? StyleValue => new StyleBuilder(Style).Build();

    /// <summary>
    /// Gets or sets the design of this input.
    /// </summary>
    [Parameter]
    public virtual FluentInputAppearance Appearance { get; set; } = FluentInputAppearance.Outline;

    /// <summary />
    protected override bool TryParseValueFromString(string? value, out DateTime? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        DateTime currentValue = Value ?? DateTime.MinValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            result = null;
        }
        else if (value != null && DateTime.TryParse(value, out var valueConverted))
        {
            result = currentValue.Date + valueConverted.TimeOfDay;
        }
        else
        {
            result = Value?.Date;
        }

        validationErrorMessage = null;
        return true;
    }
}