using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnDateField : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }

    // Chrome strings as parameters with English defaults: the library carries
    // no resources of its own, hosts pass their own words.
    [Parameter] public string TodayLabel { get; set; } = "Today";
    [Parameter] public string ClearLabel { get; set; } = "Clear";
    [Parameter] public string PickHint { get; set; } = "Pick a day";
    [Parameter] public string PreviousMonthLabel { get; set; } = "Previous month";
    [Parameter] public string NextMonthLabel { get; set; } = "Next month";

    /// <summary>Placeholder shape of the expected input, e.g. dd/mm/yyyy.</summary>
    [Parameter] public string DateFormatHint { get; set; } = "dd/mm/yyyy";


    private ElementReference _anchorRef;
    private ElementReference _popRef;
    private IJSObjectReference? _module;

    // Blur fires before the next focus, so leaving one half for the other
    // would look like leaving the control. A tick of grace tells the two
    // apart without needing relatedTarget, which Blazor does not surface.
    private int _focusDepth;

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
            // Placement is a nicety; the popover still renders in place.
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

    private CnDateInput? _input;
    private CnCalendarPanel? _calendar;
    private bool _open;
    private DateTime? _committed;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!_open)
            _committed = Value;
    }

    private Task OnValueChangedAsync(DateTime? value) => SetValueAsync(value);

    private async Task OnTypingAsync(DateTime? typed)
    {
        if (typed is { } date)
            _calendar?.ShowMonthOf(date);

        await Task.CompletedTask;
    }

    private async Task OnCommittedAsync(DateTime? value)
    {
        _committed = value;
        if (value is { } date)
            _calendar?.ShowMonthOf(date);

        await Task.CompletedTask;
    }

    private void Open()
    {
        if (_open || ReadOnly || Disabled)
            return;

        _committed = Value;
        _open = true;
    }

    private void Close() => _open = false;

    /// <summary>Escape puts back what was there before the popover opened.</summary>
    private async Task RevertAsync()
    {
        _open = false;
        if (Value != _committed)
            await SetValueAsync(_committed);
    }

    private async Task PickAsync(DateTime date)
    {
        _open = false;
        _committed = date;
        await SetValueAsync(date);
        if (_input is not null)
            await _input.FocusAsync(false);
    }

    private Task TodayAsync() => PickAsync(DateTime.Today);

    private async Task ClearAsync()
    {
        _open = false;
        _committed = null;
        await SetValueAsync(null);

        // Clearing is the start of retyping, so the caret belongs in the field.
        if (_input is not null)
            await _input.FocusAsync(false);
    }
}
