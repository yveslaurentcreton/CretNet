namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>How a timeline bar reads at a glance.</summary>
public enum CnTimelineTone
{
    /// <summary>Running, within whatever it is measured against.</summary>
    Normal,

    /// <summary>Past it.</summary>
    Over,

    /// <summary>Not started yet — drawn as an outline.</summary>
    Pending,

    /// <summary>Present but excluded; drawn dimmed, with no bar.</summary>
    Muted,
}

/// <summary>
/// One line of a <see cref="CnTimeline"/>: a span, how full it is, and up to
/// two numbers beside it.
/// </summary>
/// <remarks>
/// Deliberately data and not templates. A timeline row is a label, a span
/// and two figures in every use anybody has needed so far, and a record is
/// far easier to assert on in a test than a <c>RenderFragment</c>.
/// </remarks>
public sealed record CnTimelineRow
{
    public required string Label { get; init; }

    /// <summary>Small text after the label — a share, a code, a count.</summary>
    public string? Note { get; init; }

    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }

    /// <summary>
    /// How much of the bar is consumed, as a fraction. Values above 1 are
    /// clamped when drawn; the tone is what says it went over.
    /// </summary>
    public double Fill { get; init; }

    public CnTimelineTone Tone { get; init; } = CnTimelineTone.Normal;

    /// <summary>First figure column.</summary>
    public string? Value { get; init; }

    public string? ValueNote { get; init; }

    /// <summary>Colours the first figure without touching the bar.</summary>
    public CnTimelineTone ValueTone { get; init; } = CnTimelineTone.Normal;

    /// <summary>Second figure column, hidden wholesale by
    /// <see cref="CnTimeline.ShowSecondValue"/>.</summary>
    public string? SecondValue { get; init; }

    public string? SecondValueNote { get; init; }

    /// <summary>
    /// Thin stacked lanes of segments that replace the single bar — for
    /// example a purchase lane above a sales lane. Null or empty keeps the
    /// single bar exactly as before.
    /// </summary>
    public IReadOnlyList<CnTimelineLane>? Lanes { get; init; }

    /// <summary>
    /// A group header: the label spans the label and track columns, the
    /// figure columns stay. Group rows draw no bar and no lanes.
    /// </summary>
    public bool IsGroup { get; init; }

    /// <summary>Identifies the row for the host; compared with
    /// <see cref="CnTimeline.SelectedKey"/>.</summary>
    public object? Key { get; init; }
}

/// <summary>How a timeline segment reads at a glance. The fill says it;
/// there is deliberately no legend.</summary>
public enum CnTimelineSegmentTone
{
    /// <summary>Done — a light tint.</summary>
    Billed,

    /// <summary>Drafted but not final — a dashed light tint.</summary>
    Concept,

    /// <summary>Needs action now — solid accent.</summary>
    Due,

    /// <summary>Foreseen for later — a dashed outline, no fill.</summary>
    Later,

    /// <summary>Not covered where it should be — red hatching.</summary>
    Uncovered,

    /// <summary>The purchase side — a translucent blue tint, so overlapping
    /// segments visibly darken.</summary>
    Purchase,

    /// <summary>Anything else — a quiet grey tint.</summary>
    Neutral,

    /// <summary>An exception worth a look (a deviating price, say) — an
    /// amber tint.</summary>
    Warn,
}

/// <summary>One lane of a <see cref="CnTimelineRow"/>.</summary>
public sealed record CnTimelineLane
{
    /// <summary>A very short tag drawn beside the lane (e.g. "A", "V").</summary>
    public string? Label { get; init; }

    public IReadOnlyList<CnTimelineSegment> Segments { get; init; } = [];

    /// <summary>Draws the lane thinner, for a secondary lane.</summary>
    public bool Thin { get; init; }
}

/// <summary>
/// A period on a lane. Both dates are inclusive, so consecutive periods
/// (1–31 January, 1–28 February) tile with the same 2px gap between them.
/// Segments running past either axis edge are cut flat at that edge, unless
/// they are open-ended there.
/// </summary>
public sealed record CnTimelineSegment
{
    public required DateOnly From { get; init; }
    public required DateOnly To { get; init; }
    public CnTimelineSegmentTone Tone { get; init; } = CnTimelineSegmentTone.Billed;

    /// <summary>Tooltip and accessible name. Falls back to the date range.</summary>
    public string? Title { get; init; }

    /// <summary>A short text drawn inside the segment (a price, say), cut
    /// with an ellipsis when it does not fit. Absent, the segment is a plain
    /// bar.</summary>
    public string? Text { get; init; }

    /// <summary>The period has no start (pass <see cref="DateOnly.MinValue"/>
    /// as <see cref="From"/>): its left edge is dotted instead of cut flat.</summary>
    public bool OpenStart { get; init; }

    /// <summary>The period has no end (pass <see cref="DateOnly.MaxValue"/>
    /// as <see cref="To"/>): its right edge is dotted instead of cut flat.</summary>
    public bool OpenEnd { get; init; }

    /// <summary>Identifies the segment for the host; compared with
    /// <see cref="CnTimeline.SelectedKey"/>.</summary>
    public object? Key { get; init; }
}

/// <summary>Raised by <see cref="CnTimeline.OnSegmentClick"/>.</summary>
public sealed record CnTimelineSegmentClick(
    CnTimelineRow Row,
    CnTimelineLane Lane,
    CnTimelineSegment Segment);
