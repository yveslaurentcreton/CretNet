namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// The extent of a scrollable <see cref="CnTimeline"/> (HCMT M-064): back
/// to the first date there is, forward to a year past today or past the last
/// date, whichever is later. Hosts pass the dates that matter to them (the
/// first price, the first period, a purchase); open ends are left out and
/// simply run to the edge.
/// </summary>
public static class CnTimelineAxis
{
    /// <summary>Months shown ahead of the last date, or of today.</summary>
    public const int MonthsAhead = 12;

    /// <summary>
    /// Months kept before today even without older dates, so a one-year
    /// view can start with today a third from the left.
    /// </summary>
    public const int MonthsBeforeToday = 4;

    /// <summary>
    /// From the first of the earliest month (at least
    /// <see cref="MonthsBeforeToday"/> before today) to the last day of the
    /// month <see cref="MonthsAhead"/> months after the later of today and
    /// the last date. <see cref="DateOnly.MinValue"/> and
    /// <see cref="DateOnly.MaxValue"/> stand for open ends and are ignored.
    /// </summary>
    public static (DateOnly From, DateOnly To) Extent(IEnumerable<DateOnly> dates, DateOnly today)
    {
        var first = today.AddMonths(-MonthsBeforeToday);
        var last = today;
        foreach (var date in dates)
        {
            if (date == DateOnly.MinValue || date == DateOnly.MaxValue)
                continue;
            if (date < first)
                first = date;
            if (date > last)
                last = date;
        }

        var end = new DateOnly(last.Year, last.Month, 1).AddMonths(MonthsAhead + 1).AddDays(-1);
        return (new DateOnly(first.Year, first.Month, 1), end);
    }
}
