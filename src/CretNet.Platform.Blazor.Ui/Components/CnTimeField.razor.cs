using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnTimeField : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    /// <summary>Ask for seconds and the whole control becomes hh:mm:ss.</summary>
    [Parameter] public bool Seconds { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    // Chrome strings as parameters with English defaults.
    [Parameter] public string NowLabel { get; set; } = "Now";
    [Parameter] public string ClearLabel { get; set; } = "Clear";
    [Parameter] public string HourHint { get; set; } = "Pick the hour";
    [Parameter] public string MinuteHint { get; set; } = "Pick the minutes";
    [Parameter] public string SecondHint { get; set; } = "Pick the seconds";

    /// <summary>Where the dial starts when the field is still empty.</summary>
    [Parameter] public TimeOnly DefaultTime { get; set; } = new(9, 0);

    private ElementReference _anchorRef;
    private ElementReference _popRef;
    private IJSObjectReference? _module;

    private CnTimeInput? _input;
    private CnClockPanel? _clock;
    private bool _open;
    private int _focusDepth;
    private TimeOnly? _committed;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!_open)
            _committed = Value;
    }

    private void OnFieldFocused(bool programmatic)
    {
        _focusDepth++;
        if (!programmatic)
            Open();
    }

    private async Task OnFieldBlurredAsync()
    {
        _focusDepth = Math.Max(0, _focusDepth - 1);
        await Task.Delay(120);
        if (_focusDepth > 0 || !_open)
            return;

        Close();
        StateHasChanged();
    }

    private void Open()
    {
        if (_open || ReadOnly || Disabled)
            return;

        _committed = Value;
        _open = true;
        _clock?.Open(Value ?? DefaultTime);
    }

    private void Close() => _open = false;

    private async Task RevertAsync()
    {
        _open = false;
        if (Value != _committed)
            await SetValueAsync(_committed);
    }

    private Task OnValueChangedAsync(TimeOnly? value) => SetValueAsync(value);

    private Task OnDialledAsync(TimeOnly value) => SetValueAsync(value);

    private async Task OnDialDoneAsync(TimeOnly value)
    {
        _open = false;
        _committed = value;
        await SetValueAsync(value);
        if (_input is not null)
            await _input.FocusAsync(false);
    }

    private async Task NowAsync()
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var value = Seconds ? now : new TimeOnly(now.Hour, now.Minute);
        _open = false;
        _committed = value;
        await SetValueAsync(value);
    }

    private async Task ClearAsync()
    {
        _open = false;
        _committed = null;
        await SetValueAsync(null);

        // Clearing is the start of retyping, so hand the caret back.
        if (_input is not null)
            await _input.FocusAsync(false);
    }

    /// <summary>Inside a dialog the popover would be clipped by the scrolling
    /// body, so while it is open it is promoted to fixed viewport coordinates.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_open)
            return;

        try
        {
            _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js");
            await _module.InvokeVoidAsync("placePanel", _popRef, _anchorRef);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
