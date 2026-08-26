using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>One entry in the quick-pick column of a <see cref="CnDateRangeField"/>.</summary>
public sealed record CnDateRangePreset(string Label, DateTime From, DateTime To);

public partial class CnDateRangeField : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public DateTime? From { get; set; }
    [Parameter] public EventCallback<DateTime?> FromChanged { get; set; }
    [Parameter] public DateTime? To { get; set; }
    [Parameter] public EventCallback<DateTime?> ToChanged { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    [Parameter] public int MonthCount { get; set; } = 2;
    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }

    /// <summary>Quick picks down the left of the calendar. Empty hides the column.</summary>
    [Parameter] public IReadOnlyList<CnDateRangePreset> Presets { get; set; } = [];

    // Chrome strings as parameters with English defaults.
    [Parameter] public string FromLabel { get; set; } = "From";
    [Parameter] public string ToLabel { get; set; } = "To";
    [Parameter] public string ClearLabel { get; set; } = "Clear";
    [Parameter] public string PickStartHint { get; set; } = "Pick a first day";
    [Parameter] public string PickEndHint { get; set; } = "(pick the last day)";
    [Parameter] public string DayLabel { get; set; } = "day";
    [Parameter] public string DaysLabel { get; set; } = "days";
    [Parameter] public string PreviousMonthLabel { get; set; } = "Previous month";
    [Parameter] public string NextMonthLabel { get; set; } = "Next month";
    [Parameter] public string DateFormatHint { get; set; } = "dd/mm/yyyy";


    private ElementReference _anchorRef;
    private ElementReference _popRef;
    private IJSObjectReference? _module;

    // Blur fires before the next focus, so leaving one half for the other
    // would look like leaving the control. A tick of grace tells the two
    // apart without needing relatedTarget, which Blazor does not surface.
    private int _focusDepth;

    private void OnFieldFocused()
    {
        _focusDepth++;
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

    private CnDateInput? _fromInput;
    private CnDateInput? _toInput;
    private CnCalendarPanel? _calendar;

    private bool _open;

    // The mouse flow: the day of the first click, and the day under the
    // cursor while the second is being chosen.
    private DateTime? _anchor;
    private DateTime? _hover;

    // What Escape puts back.
    private DateTime? _committedFrom;
    private DateTime? _committedTo;

    /// <summary>What the calendar paints: the range being dragged out with the
    /// mouse, otherwise whatever the two fields hold right now — so typing
    /// previews straight away.</summary>
    private DateTime? BandFrom => Band()?.From;

    private DateTime? BandTo => Band()?.To;

    private (DateTime From, DateTime To)? Band()
    {
        if (_anchor is { } anchor)
        {
            var other = _hover ?? anchor;
            return anchor <= other ? (anchor, other) : (other, anchor);
        }

        return (From, To) switch
        {
            ({ } from, { } to) => from <= to ? (from, to) : (to, from),
            ({ } from, null) => (from, from),
            (null, { } to) => (to, to),
            _ => null,
        };
    }

    private string DayCountText(DateTime from, DateTime to)
    {
        var days = (to.Date - from.Date).Days + 1;
        return $"{days} {(days == 1 ? DayLabel : DaysLabel)}";
    }

    protected override void OnParametersSet()
    {
        if (_open)
            return;

        _committedFrom = From;
        _committedTo = To;
    }

    private void Open()
    {
        if (_open || ReadOnly || Disabled)
            return;

        _committedFrom = From;
        _committedTo = To;
        _open = true;
        _calendar?.Reset(From);
    }

    private void Close()
    {
        _open = false;
        _anchor = null;
        _hover = null;
    }

    private async Task RevertAsync()
    {
        Close();
        if (From != _committedFrom)
            await FromChanged.InvokeAsync(_committedFrom);
        if (To != _committedTo)
            await ToChanged.InvokeAsync(_committedTo);
    }

    // ----- typing -----

    private Task OnFromChangedAsync(DateTime? value) => FromChanged.InvokeAsync(value);

    private Task OnToChangedAsync(DateTime? value) => ToChanged.InvokeAsync(value);

    private Task OnFromTypingAsync(DateTime? typed)
    {
        _anchor = null;
        if (typed is { } date)
            _calendar?.ShowMonthOf(date);
        return Task.CompletedTask;
    }

    private Task OnToTypingAsync(DateTime? typed)
    {
        _anchor = null;
        if (typed is { } date)
            _calendar?.ShowMonthOf(date);
        return Task.CompletedTask;
    }

    private Task OnFromCommittedAsync(DateTime? value) => NormaliseAsync(value, To);

    private Task OnToCommittedAsync(DateTime? value) => NormaliseAsync(From, value);

    /// <summary>A pair typed the wrong way round is still a period: the
    /// earliest day becomes the start.</summary>
    private async Task NormaliseAsync(DateTime? from, DateTime? to)
    {
        if (from is not { } start || to is not { } end || start <= end)
            return;

        await FromChanged.InvokeAsync(end);
        await ToChanged.InvokeAsync(start);
    }

    /// <summary>A ninth digit belongs to the second half of the range.</summary>
    private async Task OnOverflowAsync(string digits)
    {
        if (_toInput is null)
            return;

        await _toInput.FocusAsync(false);
        await _toInput.TakeOverflowAsync(digits);
    }

    private async Task FocusToAsync()
    {
        if (_toInput is not null)
            await _toInput.FocusAsync(true);
    }

    private async Task FocusFromAsync()
    {
        if (_fromInput is not null)
            await _fromInput.FocusAsync(false);
    }

    // ----- mouse -----

    private async Task PickAsync(DateTime date)
    {
        if (_anchor is null)
        {
            _anchor = date;
            _hover = date;
            await FromChanged.InvokeAsync(date);
            await ToChanged.InvokeAsync(null);
            if (_toInput is not null)
                await _toInput.FocusAsync(false);
            return;
        }

        var start = _anchor.Value <= date ? _anchor.Value : date;
        var end = _anchor.Value <= date ? date : _anchor.Value;

        Close();
        _committedFrom = start;
        _committedTo = end;
        await FromChanged.InvokeAsync(start);
        await ToChanged.InvokeAsync(end);
    }

    private Task HoverAsync(DateTime date)
    {
        if (_anchor is not null)
            _hover = date;

        return Task.CompletedTask;
    }

    private async Task ApplyPresetAsync(CnDateRangePreset preset)
    {
        Close();
        _committedFrom = preset.From.Date;
        _committedTo = preset.To.Date;
        await FromChanged.InvokeAsync(preset.From.Date);
        await ToChanged.InvokeAsync(preset.To.Date);
        _calendar?.ShowMonthOf(preset.From.Date);
    }

    private async Task ClearAsync()
    {
        Close();
        _committedFrom = null;
        _committedTo = null;
        await FromChanged.InvokeAsync(null);
        await ToChanged.InvokeAsync(null);

        // Clearing is the start of retyping, so the caret belongs in the
        // first half — not nowhere, which is where the button click left it.
        if (_fromInput is not null)
            await _fromInput.FocusAsync(false);
    }
}
