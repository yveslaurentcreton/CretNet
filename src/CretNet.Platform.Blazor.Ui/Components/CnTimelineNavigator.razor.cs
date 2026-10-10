using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>What a scrollable <see cref="CnTimeline"/> has in view: month
/// indexes counted from the axis start, and whether either end is reached.</summary>
public readonly record struct CnTimelineView(int FirstMonth, int LastMonth, bool AtStart, bool AtEnd);

/// <summary>
/// The range, ‹ Vandaag › and the zoom above a scrollable
/// <see cref="CnTimeline"/>. Rendered by the timeline itself; not meant to be
/// placed on its own.
/// </summary>
public partial class CnTimelineNavigator
{
    /// <summary>The view before the browser has reported one.</summary>
    [Parameter] public CnTimelineView Initial { get; set; }

    /// <summary>"jun 2026 – mei 2027" for a view.</summary>
    [Parameter, EditorRequired] public Func<CnTimelineView, string> Range { get; set; } = _ => string.Empty;

    [Parameter] public int Zoom { get; set; }
    [Parameter] public IReadOnlyList<int> Zooms { get; set; } = [];
    [Parameter] public EventCallback<int> OnZoom { get; set; }

    /// <summary>-1 half a view earlier, 0 today, 1 half a view later.</summary>
    [Parameter] public EventCallback<int> OnGo { get; set; }

    private CnTimelineView? _reported;

    private CnTimelineView View => _reported ?? Initial;

    private string RangeText => Range(View);

    /// <summary>What the browser reports while scrolling; re-renders only this bar.</summary>
    public void Show(CnTimelineView view)
    {
        if (_reported == view)
            return;

        _reported = view;
        StateHasChanged();
    }

    /// <summary>Forgets the reported view, after the axis itself changed.</summary>
    public void Reset() => _reported = null;

    /// <summary>"6 mnd", "1 jaar", "3 jaar", "Alles".</summary>
    public static string ZoomText(int months) => months switch
    {
        <= 0 => CnLabels.All,
        12 => CnLabels.TimelineOneYear,
        _ when months % 12 == 0 => CnLabels.Format(CnLabels.TimelineYears, months / 12),
        _ => CnLabels.Format(CnLabels.TimelineMonths, months),
    };
}
