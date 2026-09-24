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

    // Resource-backed chrome strings, optionally overridden by the consumer.
    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    [Parameter] public string LabelHeader { get => field ?? CnLabels.Item; set; } = null!;
    [Parameter] public string ValueHeader { get => field ?? CnLabels.Value; set; } = null!;
    [Parameter] public string SecondValueHeader { get => field ?? CnLabels.Amount; set; } = null!;
    [Parameter] public string EmptyText { get => field ?? CnLabels.EmptyValue; set; } = null!;
    #pragma warning restore BL0007

    /// <summary>Month abbreviations, left to right. Supplying them keeps the
    /// component out of the business of guessing a culture.</summary>
    [Parameter] public IReadOnlyList<string>? MonthNames { get; set; }

    [Parameter] public string? Class { get; set; }

    /// <summary>Makes every row clickable (mouse on the whole row, keyboard
    /// on its label button). Group rows included.</summary>
    [Parameter] public EventCallback<CnTimelineRow> OnRowClick { get; set; }

    /// <summary>Makes lane segments buttons. A segment click does not also
    /// raise <see cref="OnRowClick"/>.</summary>
    [Parameter] public EventCallback<CnTimelineSegmentClick> OnSegmentClick { get; set; }

    /// <summary>Highlights the row and/or segment whose <c>Key</c> equals
    /// this value.</summary>
    [Parameter] public object? SelectedKey { get; set; }

    private double AxisDays => Math.Max(1d, To.DayNumber - From.DayNumber);

    private double? TodayPercentage =>
        Today is { } today && today >= From && today <= To
            ? (today.DayNumber - From.DayNumber) / AxisDays * 100d
            : null;

    private IEnumerable<(string Label, double Percentage, bool Year, bool Alternate)> Ticks
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
                // January carries the year: it is where the reader needs it,
                // and a year label on every tick would crowd out the months.
                // Even months are the ones that give way when narrow, so the
                // year never loses its label and never has a neighbour.
                var year = cursor.Month == 1;
                yield return (year ? cursor.Year.ToString(CultureInfo.InvariantCulture) : names[(cursor.Month - 1) % names.Count],
                              (at.DayNumber - From.DayNumber) / AxisDays * 100d,
                              year,
                              cursor.Month % 2 == 0);

                cursor = cursor.AddMonths(1);
                months++;
            }
        }
    }

    /// <summary>Months on the axis, for the gridline pitch.</summary>
    private string MonthCount => Css(Math.Max(1d, AxisDays / (365.2425d / 12d)));

    /// <summary>Beyond about fifteen months the alternate labels go earlier.</summary>
    private bool Dense => AxisDays > 460d;

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

    private bool HasLanes(CnTimelineRow row) => !row.IsGroup && row.Lanes is { Count: > 0 };

    /// <summary>Rows using none of the lane/group/click features keep the
    /// original flat markup, cell for cell.</summary>
    private bool IsEnhanced(CnTimelineRow row) => row.IsGroup || HasLanes(row) || OnRowClick.HasDelegate;

    private bool IsSelected(object? key) => SelectedKey is not null && key is not null && Equals(key, SelectedKey);

    /// <summary>
    /// Null when the segment misses the axis. Dates are inclusive, so the
    /// right edge is the day after <c>To</c>; a segment is cut when it runs
    /// past an axis edge, and loses its 1px inset on that side.
    /// </summary>
    private (double Left, double Width, bool CutLeft, bool CutRight)? Place(CnTimelineSegment segment)
    {
        if (To <= From || (segment.To < segment.From) || (segment.To < From) || (segment.From > To))
        {
            return null;
        }

        var left = Math.Clamp((segment.From.DayNumber - From.DayNumber) / AxisDays * 100d, 0d, 100d);
        var right = Math.Clamp((segment.To.DayNumber + 1 - From.DayNumber) / AxisDays * 100d, 0d, 100d);

        var cutLeft = segment.From.DayNumber < From.DayNumber;
        var cutRight = segment.To.DayNumber > To.DayNumber;

        return (left, right - left, cutLeft, cutRight);
    }

    private static string SegmentStyle((double Left, double Width, bool CutLeft, bool CutRight) place)
    {
        var inset = place.CutLeft ? 0 : 1;
        var gaps = inset + (place.CutRight ? 0 : 1);
        return $"left: calc({Css(place.Left)}% + {inset}px); width: max(2px, calc({Css(place.Width)}% - {gaps}px))";
    }

    private string SegmentClass(CnTimelineSegment segment, bool thin, (double Left, double Width, bool CutLeft, bool CutRight) place) =>
        string.Join(' ', new[]
        {
            "cn-gantt-seg",
            SegmentToneClass(segment.Tone),
            thin ? "cn-gantt-seg--thin" : null,
            place.CutLeft ? "cn-gantt-seg--cut-l" : null,
            place.CutRight ? "cn-gantt-seg--cut-r" : null,
            IsSelected(segment.Key) ? "cn-gantt-seg--selected" : null,
        }.Where(x => x is not null));

    private static string SegmentToneClass(CnTimelineSegmentTone tone) => tone switch
    {
        CnTimelineSegmentTone.Concept => "cn-gantt-seg--concept",
        CnTimelineSegmentTone.Due => "cn-gantt-seg--due",
        CnTimelineSegmentTone.Later => "cn-gantt-seg--later",
        CnTimelineSegmentTone.Uncovered => "cn-gantt-seg--uncovered",
        CnTimelineSegmentTone.Purchase => "cn-gantt-seg--purchase",
        CnTimelineSegmentTone.Neutral => "cn-gantt-seg--neutral",
        _ => "cn-gantt-seg--billed",
    };

    private static string SegmentTitle(CnTimelineSegment segment) =>
        string.IsNullOrWhiteSpace(segment.Title)
            ? $"{segment.From.ToString("d", CultureInfo.CurrentCulture)} – {segment.To.ToString("d", CultureInfo.CurrentCulture)}"
            : segment.Title;

    private string RowClass(CnTimelineRow row) =>
        string.Join(' ', new[]
        {
            "cn-gantt-row",
            row.IsGroup ? "cn-gantt-row--group" : null,
            HasLanes(row) ? "cn-gantt-row--lanes" : null,
            OnRowClick.HasDelegate ? "cn-gantt-row--click" : null,
            IsSelected(row.Key) ? "cn-gantt-row--selected" : null,
        }.Where(x => x is not null));

    private Task RowClicked(CnTimelineRow row) =>
        OnRowClick.HasDelegate ? OnRowClick.InvokeAsync(row) : Task.CompletedTask;

    private Task SegmentClicked(CnTimelineRow row, CnTimelineLane lane, CnTimelineSegment segment) =>
        OnSegmentClick.InvokeAsync(new CnTimelineSegmentClick(row, lane, segment));

    private static string? ToneClass(CnTimelineTone tone) => tone switch
    {
        CnTimelineTone.Over => "cn-gantt--over",
        CnTimelineTone.Pending => "cn-gantt--pending",
        CnTimelineTone.Muted => "cn-gantt--muted",
        _ => null,
    };

    private static string Css(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);
}
