using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnDateField
{
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

    private void OpenAsync()
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
    }
}
