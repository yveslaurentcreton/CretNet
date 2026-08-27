using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Spans over a shared date axis, with up to two figure columns beside them.
/// </summary>
/// <remarks>
/// <para>
/// A progress bar answers "how much is left"; this answers "and by when".
/// The two together are what turn a phase at 10&#160;% of its budget from
/// reassuring into a question — it depends entirely on whether that phase
/// runs until December or ended in March.
/// </para>
/// <para>
/// The axis is divided into equal months rather than exact day counts. A
/// February drawn the same width as a March is off by a couple of pixels;
/// gridlines that do not line up with their own labels are off by more.
/// The bars themselves are placed by real dates.
/// </para>
/// </remarks>
public partial class CnTimeline
{
    [Parameter, EditorRequired] public IReadOnlyList<CnTimelineRow> Rows { get; set; } = [];

    /// <summary>Left edge of the axis.</summary>
    [Parameter, EditorRequired] public DateOnly From { get; set; }

    /// <summary>Right edge. Anything on or before <see cref="From"/> makes
    /// the axis degenerate, and every bar is left out rather than drawn at a
    /// nonsense width.</summary>
    [Parameter, EditorRequired] public DateOnly To { get; set; }

    /// <summary>Draws the "now" marker. Absent, or outside the axis, draws
    /// nothing.</summary>
    [Parameter] public DateOnly? Today { get; set; }

    /// <summary>False drops the last column entirely — not merely its
    /// values. Used where the figures are for some readers only.</summary>
    [Parameter] public bool ShowSecondValue { get; set; } = true;

    // Chrome strings: English defaults, overridden by the consumer.
    [Parameter] public string LabelHeader { get; set; } = "Item";
    [Parameter] public string ValueHeader { get; set; } = "Value";
    [Parameter] public string SecondValueHeader { get; set; } = "Amount";
    [Parameter] public string EmptyText { get; set; } = "—";

    /// <summary>Month abbreviations, left to right. Supplying them keeps the
    /// component out of the business of guessing a culture.</summary>
    [Parameter] public IReadOnlyList<string>? MonthNames { get; set; }

    [Parameter] public string? Class { get; set; }

    private double AxisDays => Math.Max(1d, To.DayNumber - From.DayNumber);

    private double? TodayPercentage =>
        Today is { } today && today >= From && today <= To
            ? (today.DayNumber - From.DayNumber) / AxisDays * 100d
            : null;

    private IEnumerable<(string Label, double Percentage)> Ticks
    {
        get
        {
            if (To <= From)
            {
                yield break;
            }

            var names = MonthNames ?? DefaultMonthNames;
            var cursor = new DateOnly(From.Year, From.Month, 1);
            var months = 0;

            while (cursor <= To && months < 60)
            {
                var at = cursor < From ? From : cursor;
                yield return (names[(cursor.Month - 1) % names.Count],
                              (at.DayNumber - From.DayNumber) / AxisDays * 100d);

                cursor = cursor.AddMonths(1);
                months++;
            }
        }
    }

    private static readonly string[] DefaultMonthNames =
        ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];

    /// <summary>
    /// Null when the row has no dates, or none that land on the axis — a
    /// phase nobody has planned yet still gets its line and its figures, it
    /// simply has no bar to draw.
    /// </summary>
    private (double Left, double Width)? Span(CnTimelineRow row)
    {
        if (To <= From || row.Tone == CnTimelineTone.Muted)
        {
            return null;
        }

        if (row.From is not { } start || row.To is not { } end || end < start)
        {
            return null;
        }

        var left = (start.DayNumber - From.DayNumber) / AxisDays * 100d;
        var right = (end.DayNumber - From.DayNumber) / AxisDays * 100d;

        if (right < 0d || left > 100d)
        {
            return null;
        }

        left = Math.Clamp(left, 0d, 100d);
        right = Math.Clamp(right, 0d, 100d);

        // A single-day span would otherwise be invisible.
        return (left, Math.Max(1d, right - left));
    }

    private static string? ToneClass(CnTimelineTone tone) => tone switch
    {
        CnTimelineTone.Over => "cn-tl--over",
        CnTimelineTone.Pending => "cn-tl--pending",
        CnTimelineTone.Muted => "cn-tl--muted",
        _ => null,
    };

    private static string Css(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);
}
