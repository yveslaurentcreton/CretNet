using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnCalendarPanel
{
    private enum Level { Days, Months, Years }

    private const int YearBlockSize = 12;

    /// <summary>Both ends of the band to paint. A single date passes the same
    /// day twice; <c>null</c> paints nothing.</summary>
    [Parameter] public DateTime? From { get; set; }
    [Parameter] public DateTime? To { get; set; }

    [Parameter] public int MonthCount { get; set; } = 1;
    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }

    [Parameter] public EventCallback<DateTime> OnPick { get; set; }
    [Parameter] public EventCallback<DateTime> OnHover { get; set; }

    /// <summary>Line under the calendar: the resolved range, a hint, whatever
    /// the host wants to say.</summary>
    [Parameter] public RenderFragment? Readout { get; set; }
    [Parameter] public RenderFragment? FooterActions { get; set; }

    [Parameter] public string PreviousLabel { get; set; } = "Previous";
    [Parameter] public string NextLabel { get; set; } = "Next";

    private Level _level = Level.Days;
    private DateTime _view = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    private static DateTime Today => DateTime.Today;

    private static CultureInfo Culture => CultureInfo.CurrentCulture;

    private int YearBlockStart => _view.Year / YearBlockSize * YearBlockSize;

    /// <summary>Day names starting on the culture's first day of the week.</summary>
    private IEnumerable<string> DayNames
    {
        get
        {
            var first = (int)Culture.DateTimeFormat.FirstDayOfWeek;
            for (var i = 0; i < 7; i++)
            {
                var day = (DayOfWeek)((first + i) % 7);
                var name = Culture.DateTimeFormat.GetShortestDayName(day);
                yield return name.Length > 2 ? name[..2] : name;
            }
        }
    }

    /// <summary>The popover must not resize when zooming, so the body keeps
    /// the width of its months and the height of six week rows.</summary>
    private string BodyStyle =>
        $"min-width: {224 * MonthCount + 22 * (MonthCount - 1)}px;";

    private int LeadingBlanks(DateTime month)
    {
        var first = new DateTime(month.Year, month.Month, 1);
        var offset = (int)first.DayOfWeek - (int)Culture.DateTimeFormat.FirstDayOfWeek;
        return (offset + 7) % 7;
    }

    /// <summary>Shows the month a value lives in, and drops back to the day
    /// level: a zoom level is a way to navigate, not a state worth keeping.</summary>
    public void ShowMonthOf(DateTime date)
    {
        var last = _view.AddMonths(MonthCount).AddDays(-1);
        if (_level != Level.Days || date < _view || date > last)
        {
            _view = new DateTime(date.Year, date.Month, 1);
            _level = Level.Days;
            StateHasChanged();
        }
    }

    /// <summary>Called when the popover opens, so it always starts on days.</summary>
    public void Reset(DateTime? focus)
    {
        _level = Level.Days;
        var anchor = focus ?? From ?? Today;
        _view = new DateTime(anchor.Year, anchor.Month, 1);
        StateHasChanged();
    }

    /// <summary>The arrows step by whatever the current level shows: a month,
    /// a year, or a whole block of years.</summary>
    private void Shift(int delta) => _view = _level switch
    {
        Level.Days => _view.AddMonths(delta),
        Level.Months => _view.AddYears(delta),
        _ => _view.AddYears(delta * YearBlockSize),
    };

    private void ZoomOut(DateTime shown)
    {
        _view = new DateTime(shown.Year, shown.Month, 1);
        _level = Level.Months;
    }

    private void ZoomIn(DateTime month)
    {
        _view = month;
        _level = Level.Days;
    }

    private bool IsDisabled(DateTime date) =>
        (MinDate is { } min && date < min.Date) || (MaxDate is { } max && date > max.Date);

    private bool IsEnd(DateTime date) =>
        (From is { } from && date == from.Date) || (To is { } to && date == to.Date);

    /// <summary>The band is drawn per cell so the tint runs on uninterrupted
    /// across a week while every day stays a circle. Cells round off where the
    /// band actually stops: at the ends, and at the edges of each week row.</summary>
    private string? BandClass(DateTime date)
    {
        if (From is not { } from || To is not { } to)
            return null;

        var start = from.Date <= to.Date ? from.Date : to.Date;
        var end = from.Date <= to.Date ? to.Date : from.Date;
        if (date < start || date > end)
            return null;

        if (start == end)
            return "cn-cal-cell--in cn-cal-cell--solo";

        var classes = "cn-cal-cell--in";
        var column = ((int)date.DayOfWeek - (int)Culture.DateTimeFormat.FirstDayOfWeek + 7) % 7;
        var lastOfMonth = DateTime.DaysInMonth(date.Year, date.Month);

        if (date == start || column == 0 || date.Day == 1)
            classes += " cn-cal-cell--edge-start";
        if (date == end || column == 6 || date.Day == lastOfMonth)
            classes += " cn-cal-cell--edge-end";

        return classes;
    }

    private bool IsSelectedMonth(int year, int month) =>
        From is { } from && from.Year == year && from.Month == month;

    private bool IsCurrentMonth(int year, int month) =>
        Today.Year == year && Today.Month == month;

    private bool IsSelectedYear(int year) => From is { } from && from.Year == year;
}
