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
}
