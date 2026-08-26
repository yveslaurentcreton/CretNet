using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnDateTimeField : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public DateTime? Value { get; set; }
    [Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }

    /// <summary>Ask for seconds and the time half becomes hh:mm:ss.</summary>
    [Parameter] public bool Seconds { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }

    /// <summary>Time used when a date is picked before any time was given.</summary>
    [Parameter] public TimeOnly DefaultTime { get; set; } = new(9, 0);

    // Chrome strings as parameters with English defaults.
    [Parameter] public string DateLabel { get; set; } = "Date";
    [Parameter] public string TimeLabel { get; set; } = "Time";
    [Parameter] public string NowLabel { get; set; } = "Now";
    [Parameter] public string ClearLabel { get; set; } = "Clear";
    [Parameter] public string PickHint { get; set; } = "Pick a day";
    [Parameter] public string HourHint { get; set; } = "Pick the hour";
    [Parameter] public string MinuteHint { get; set; } = "Pick the minutes";
    [Parameter] public string SecondHint { get; set; } = "Pick the seconds";
    [Parameter] public string PreviousMonthLabel { get; set; } = "Previous month";
    [Parameter] public string NextMonthLabel { get; set; } = "Next month";
    [Parameter] public string DateFormatHint { get; set; } = "dd/mm/yyyy";

    private ElementReference _anchorRef;
    private ElementReference _popRef;
    private IJSObjectReference? _module;

    private CnDateInput? _dateInput;
    private CnTimeInput? _timeInput;
    private CnCalendarPanel? _calendar;
    private CnClockPanel? _clock;

    private bool _open;
    private bool _showClock;
    private int _focusDepth;
    private DateTime? _committed;

    // The two halves of the value. A date without a time is still worth
    // holding on to while the user is halfway through filling the pair in.
    private DateTime? _date;
    private TimeOnly? _time;

    private DateTime? DatePart => _date;

    private TimeOnly? TimePart => _time;

    protected override void OnParametersSet()
    {
        if (_open)
            return;

        _committed = Value;
        _date = Value?.Date;
        _time = Value is { } value ? TimeOnly.FromDateTime(value) : null;
    }

    private string Formatted(DateTime value) =>
        CnDateMask.Format(value.Date) + " " + CnTimeMask.Format(TimeOnly.FromDateTime(value), Seconds);

    /// <summary>Both halves make one value; a date on its own waits for a time
    /// rather than guessing one.</summary>
    private async Task PushAsync()
    {
        var next = _date is { } date && _time is { } time ? date.Date + time.ToTimeSpan() : (DateTime?)null;
        if (next == Value)
            return;

        await ValueChanged.InvokeAsync(next);
    }

    // ----- focus and opening -----

    private void OnDateFocused(bool programmatic)
    {
        _focusDepth++;
        if (!programmatic)
            OpenCalendar();
    }

    private void OnTimeFocused(bool programmatic)
    {
        _focusDepth++;
        if (!programmatic)
            OpenClock();
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

    private void OpenCalendar()
    {
        if (ReadOnly || Disabled)
            return;

        if (!_open)
            _committed = Value;

        _open = true;
        _showClock = false;
    }

    private void OpenClock()
    {
        if (ReadOnly || Disabled)
            return;

        if (!_open)
            _committed = Value;

        _open = true;
        _showClock = true;
        _clock?.Open(_time ?? DefaultTime);
    }

    private void BackToCalendar() => _showClock = false;

    private void Close()
    {
        _open = false;
        _showClock = false;
    }

    private async Task RevertAsync()
    {
        Close();
        if (Value != _committed)
            await ValueChanged.InvokeAsync(_committed);
    }

    // ----- typing -----

    private async Task OnDateChangedAsync(DateTime? date)
    {
        _date = date;
        await PushAsync();
    }

    private Task OnDateTypingAsync(DateTime? typed)
    {
        if (typed is { } date)
            _calendar?.ShowMonthOf(date);

        return Task.CompletedTask;
    }

    /// <summary>A finished date has nothing left to type: the caret moves on to
    /// the time and the popover turns into the clock, exactly as picking a day
    /// in the calendar does.</summary>
    private async Task OnDateFilledAsync()
    {
        if (_timeInput is null)
            return;

        await _timeInput.FocusAsync(true);
        OpenClock();
    }

    private async Task OnTimeChangedAsync(TimeOnly? time)
    {
        _time = time;
        await PushAsync();
    }

    private async Task FocusTimeAsync()
    {
        if (_timeInput is not null)
            await _timeInput.FocusAsync(true);
    }

    private async Task FocusDateAsync()
    {
        if (_dateInput is null)
            return;

        await _dateInput.FocusAsync(false);
        OpenCalendar();
    }

    // ----- picking -----

    private async Task PickDateAsync(DateTime date)
    {
        _date = date;
        _time ??= DefaultTime;
        await PushAsync();

        // The date is settled; move on to the time rather than closing on a
        // half-finished value.
        if (_timeInput is not null)
            await _timeInput.FocusAsync(true);

        OpenClock();
    }

    private async Task OnDialledAsync(TimeOnly time)
    {
        _time = time;
        await PushAsync();
    }

    private async Task OnDialDoneAsync(TimeOnly time)
    {
        _time = time;
        await PushAsync();
        Close();
        _committed = Value;

        if (_timeInput is not null)
            await _timeInput.FocusAsync(false);
    }

    private async Task NowAsync()
    {
        var now = DateTime.Now;
        _date = now.Date;
        _time = Seconds
            ? TimeOnly.FromDateTime(now)
            : new TimeOnly(now.Hour, now.Minute);

        await PushAsync();
        Close();
        _committed = Value;
    }

    private async Task ClearAsync()
    {
        Close();
        _date = null;
        _time = null;
        _committed = null;
        await ValueChanged.InvokeAsync(null);

        if (_dateInput is not null)
            await _dateInput.FocusAsync(false);
    }

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
