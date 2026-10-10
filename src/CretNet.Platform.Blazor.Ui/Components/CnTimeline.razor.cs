using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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
/// <para>
/// <see cref="Scrollable"/> (HCMT M-064) gives every month the same pixel
/// width — the visible track divided by <see cref="VisibleMonths"/> — and
/// lets the track scroll under a sticky name column and sticky figure
/// columns. The axis then runs from the first of <see cref="From"/>'s month
/// to the last of <see cref="To"/>'s, and dates are placed within their
/// month, so gridlines, month labels and bars line up exactly. The widths
/// are CSS; <c>CnTimeline.razor.js</c> only moves and reports the scroll
/// position.
/// </para>
/// </remarks>
public partial class CnTimeline : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnTimeline.razor.js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

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

    /// <summary>False drops both figure columns, for a timeline with no
    /// figures beside it.</summary>
    [Parameter] public bool ShowValue { get; set; } = true;

    /// <summary>
    /// Scrolls through time instead of squeezing the axis into the width:
    /// equal months of the visible width / <see cref="VisibleMonths"/>, a
    /// range label, ‹ Vandaag › and a zoom above it, drag, swipe, Shift +
    /// wheel and the arrow keys to move. It starts with today a third from
    /// the left. False renders exactly the classic timeline.
    /// </summary>
    [Parameter] public bool Scrollable { get; set; }

    /// <summary>Months in view when <see cref="Scrollable"/>; 0 shows the
    /// whole axis without scrolling. The user's zoom replaces it.</summary>
    [Parameter] public int VisibleMonths { get; set; } = 12;

    /// <summary>Remembers the user's zoom in the browser under this key (one
    /// per kind of timeline); the position is never remembered. Null
    /// remembers nothing.</summary>
    [Parameter] public string? ZoomStorageKey { get; set; }

    /// <summary>The zoom choices: 6 months, 1 year, 3 years and everything (0).</summary>
    public static IReadOnlyList<int> Zooms { get; } = [6, 12, 36, 0];

    private CnTimelineNavigator? _navigator;
    private ElementReference _scroller;
    private IJSObjectReference? _module;
    private DotNetObjectReference<CnTimeline>? _self;
    private int? _visibleParameter;
    private int _zoom = 12;
    private bool _zoomRead;
    private bool _attached;
    private bool _todayPending;
    private (DateOnly Start, DateOnly End) _axis;

    protected override void OnParametersSet()
    {
        if (_visibleParameter != VisibleMonths)
        {
            _visibleParameter = VisibleMonths;
            _zoom = Math.Max(0, VisibleMonths);
        }

        var axis = (Start, End);
        if (axis != _axis)
        {
            // A new axis puts today back a third from the left.
            _axis = axis;
            _todayPending = _attached;
            _navigator?.Reset();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!Scrollable)
        {
            if (_attached)
                await DetachAsync();
            return;
        }

        try
        {
            if (!_attached)
            {
                _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
                if (!_zoomRead && ZoomStorageKey is { Length: > 0 } key)
                {
                    _zoomRead = true;
                    if (await _module.InvokeAsync<int?>("readZoom", key) is { } stored && Zooms.Contains(stored) && stored != _zoom)
                    {
                        // Render the remembered zoom first; attach after it.
                        _zoom = stored;
                        StateHasChanged();
                        return;
                    }
                }

                _self ??= DotNetObjectReference.Create(this);
                await _module.InvokeVoidAsync("attach", _scroller, _self);
                _attached = true;
                _todayPending = true;
            }

            if (_todayPending)
            {
                _todayPending = false;
                await _module!.InvokeVoidAsync("scrollToToday", _scroller, false);
            }
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException exception)
        {
            // The timeline still renders and scrolls natively without the module.
            Console.Error.WriteLine($"CnTimeline scrolling failed: {exception.Message}");
        }
    }

    /// <summary>What the browser reports while scrolling (month indexes
    /// from the axis start); re-renders only the navigator.</summary>
    [JSInvokable]
    public Task OnViewChanged(int firstMonth, int lastMonth, bool atStart, bool atEnd) =>
        InvokeAsync(() => _navigator?.Show(new CnTimelineView(firstMonth, lastMonth, atStart, atEnd)));

    private async Task ZoomAsync(int months)
    {
        _zoom = Math.Max(0, months);
        if (_module is null || ZoomStorageKey is not { Length: > 0 } key)
            return;

        try
        {
            await _module.InvokeVoidAsync("writeZoom", key, _zoom);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
            // Not remembered; the zoom itself still applies.
        }
    }

    private async Task GoAsync(int direction)
    {
        if (_module is null || !_attached)
            return;

        try
        {
            if (direction == 0)
                await _module.InvokeVoidAsync("scrollToToday", _scroller, true);
            else
                await _module.InvokeVoidAsync("page", _scroller, direction);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
        }
    }

    private async Task DetachAsync()
    {
        _attached = false;
        if (_module is null)
            return;

        try
        {
            await _module.InvokeVoidAsync("detach", _scroller);
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
        if (_module is not null)
        {
            if (_attached)
                await DetachAsync();

            try
            {
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }

        _self?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>The axis start: <see cref="From"/>, or the first of its
    /// month when scrollable.</summary>
    private DateOnly Start => Scrollable ? new DateOnly(From.Year, From.Month, 1) : From;

    /// <summary>The axis end: <see cref="To"/>, or the last of its month
    /// when scrollable.</summary>
    private DateOnly End => Scrollable ? new DateOnly(To.Year, To.Month, 1).AddMonths(1).AddDays(-1) : To;

    private double AxisDays => Math.Max(1d, End.DayNumber - Start.DayNumber);

    /// <summary>Whole months on a scrollable axis.</summary>
    private int Months => Math.Max(1, MonthIndex(End) - MonthIndex(Start) + 1);

    /// <summary>Months in view: the zoom, or the whole axis for "everything"
    /// and for an axis shorter than the zoom.</summary>
    private int EffectiveVisible => _zoom <= 0 || _zoom >= Months ? Months : _zoom;

    private static int MonthIndex(DateOnly date) => date.Year * 12 + date.Month - 1;

    /// <summary>Where a day starts on the axis, 0..1 inside it. Scrollable:
    /// within its month, so every month is the same width.</summary>
    private double Fraction(DateOnly date) =>
        Scrollable
            ? (MonthIndex(date) - MonthIndex(Start) + (date.Day - 1) / (double)DateTime.DaysInMonth(date.Year, date.Month)) / Months
            : (date.DayNumber - Start.DayNumber) / AxisDays;

    /// <summary>Where an inclusive last day ends on the axis.</summary>
    private double EndFraction(DateOnly date) =>
        Scrollable
            ? (MonthIndex(date) - MonthIndex(Start) + date.Day / (double)DateTime.DaysInMonth(date.Year, date.Month)) / Months
            : (date.DayNumber + 1 - Start.DayNumber) / AxisDays;

    private double? TodayPercentage =>
        Today is { } today && today >= Start && today <= End
            ? Fraction(today) * 100d
            : null;

    private IEnumerable<(string Label, double Percentage, bool Year, bool Alternate)> Ticks
    {
        get
        {
            if (End <= Start)
            {
                yield break;
            }

            var names = MonthNames ?? DefaultMonthNames;
            var cursor = new DateOnly(Start.Year, Start.Month, 1);
            var months = 0;

            while (cursor <= End && months < 60)
            {
                var at = cursor < Start ? Start : cursor;
                // January carries the year: it is where the reader needs it,
                // and a year label on every tick would crowd out the months.
                // Even months are the ones that give way when narrow, so the
                // year never loses its label and never has a neighbour.
                var year = cursor.Month == 1;
                yield return (year ? cursor.Year.ToString(CultureInfo.InvariantCulture) : names[(cursor.Month - 1) % names.Count],
                              (at.DayNumber - Start.DayNumber) / AxisDays * 100d,
                              year,
                              cursor.Month % 2 == 0);

                cursor = cursor.AddMonths(1);
                months++;
            }
        }
    }

    /// <summary>
    /// Every month of a scrollable axis, each named: the years have their own
    /// row above. January draws the stronger line, even months give way on a
    /// crowded axis, then all but the quarters, and the current month is
    /// marked.
    /// </summary>
    private IEnumerable<(string Label, double Left, double Width, string Class)> MonthTicks
    {
        get
        {
            var names = MonthNames ?? DefaultMonthNames;
            var width = 100d / Months;
            for (var index = 0; index < Months; index++)
            {
                var month = Start.AddMonths(index);
                var now = Today is { } today && today.Year == month.Year && today.Month == month.Month;
                var css = string.Join(' ', new[]
                {
                    "cn-gantt-mo",
                    month.Month == 1 ? "cn-gantt-mo--year" : null,
                    month.Month % 2 == 0 ? "cn-gantt-mo--alt" : null,
                    month.Month % 3 == 1 ? null : "cn-gantt-mo--minor",
                    now ? "cn-gantt-mo--now" : null,
                }.Where(x => x is not null));
                yield return (names[(month.Month - 1) % names.Count], index * width, width, css);
            }
        }
    }

    /// <summary>A band per year on a scrollable axis; its label sticks to the
    /// left of the view while any of the year is in it.</summary>
    private IEnumerable<(int Year, double Left, double Width)> Years
    {
        get
        {
            var first = MonthIndex(Start);
            for (var year = Start.Year; year <= End.Year; year++)
            {
                var from = Math.Max(0, year * 12 - first);
                var to = Math.Min(Months, (year + 1) * 12 - first);
                if (to > from)
                {
                    yield return (year, from * 100d / Months, (to - from) * 100d / Months);
                }
            }
        }
    }

    /// <summary>Months on the axis, for the gridline pitch.</summary>
    private string MonthCount => Css(Math.Max(1d, AxisDays / (365.2425d / 12d)));

    /// <summary>Beyond about fifteen months the alternate labels go earlier.</summary>
    private bool Dense => !Scrollable && AxisDays > 460d;

    private string? ModeClass
    {
        get
        {
            var classes = new[]
            {
                ShowValue ? null : "cn-gantt--novalue",
                Scrollable ? "cn-gantt--scroll" : null,
                !Scrollable ? null : EffectiveVisible switch
                {
                    <= 15 => null,
                    <= 30 => "cn-gantt--labels-half",
                    <= 72 => "cn-gantt--labels-quarter",
                    _ => "cn-gantt--labels-year",
                },
            }.Where(x => x is not null);
            var joined = string.Join(' ', classes);
            return joined.Length == 0 ? null : joined;
        }
    }

    /// <summary>Scrollable: whole months, the months in view and how many
    /// months the first January is in, for the yearly gridline.</summary>
    private string GridStyle =>
        Scrollable
            ? $"--cn-gantt-months: {Months}; --cn-gantt-visible: {EffectiveVisible}; --cn-gantt-year-at: {(13 - Start.Month) % 12}"
            : $"--cn-gantt-months: {MonthCount}";

    private int? MonthsData => Scrollable ? Months : null;

    /// <summary>Where today is on a scrollable axis (0..1), for the start position.</summary>
    private string? TodayData =>
        Scrollable && Today is { } today
            ? Math.Clamp(Fraction(today), 0d, 1d).ToString("0.######", CultureInfo.InvariantCulture)
            : null;

    /// <summary>The view before the browser reports one: today a third from
    /// the left, like the start position the module scrolls to.</summary>
    private CnTimelineView InitialView
    {
        get
        {
            var visible = EffectiveVisible;
            var first = Today is { } today
                ? (int)Math.Round(Math.Clamp(Fraction(today), 0d, 1d) * Months - visible / 3d, MidpointRounding.AwayFromZero)
                : 0;
            first = Math.Clamp(first, 0, Months - visible);
            var last = first + visible - 1;
            return new CnTimelineView(first, last, first == 0, last >= Months - 1);
        }
    }

    /// <summary>"jun 2026 – mei 2027".</summary>
    private string RangeText(CnTimelineView view)
    {
        var names = MonthNames ?? DefaultMonthNames;
        var first = Start.AddMonths(Math.Clamp(view.FirstMonth, 0, Months - 1));
        var last = Start.AddMonths(Math.Clamp(view.LastMonth, 0, Months - 1));
        return $"{names[(first.Month - 1) % names.Count]} {first.Year.ToString(CultureInfo.InvariantCulture)} – {names[(last.Month - 1) % names.Count]} {last.Year.ToString(CultureInfo.InvariantCulture)}";
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
        if (End <= Start || row.Tone == CnTimelineTone.Muted)
        {
            return null;
        }

        if (row.From is not { } start || row.To is not { } end || end < start)
        {
            return null;
        }

        var left = Fraction(start) * 100d;
        var right = (Scrollable ? EndFraction(end) : Fraction(end)) * 100d;

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
    /// original flat markup, cell for cell. A scrollable timeline wraps
    /// every row, so its figure cells can stick to the right.</summary>
    private bool IsEnhanced(CnTimelineRow row) => Scrollable || row.IsGroup || HasLanes(row) || OnRowClick.HasDelegate;

    private bool IsSelected(object? key) => SelectedKey is not null && key is not null && Equals(key, SelectedKey);

    /// <summary>
    /// Null when the segment misses the axis. Dates are inclusive, so the
    /// right edge is the day after <c>To</c>; a segment is cut when it runs
    /// past an axis edge, and loses its 1px inset on that side.
    /// </summary>
    private (double Left, double Width, bool CutLeft, bool CutRight)? Place(CnTimelineSegment segment)
    {
        if (End <= Start || (segment.To < segment.From) || (segment.To < Start) || (segment.From > End))
        {
            return null;
        }

        var left = Math.Clamp(Fraction(segment.From) * 100d, 0d, 100d);
        var right = Math.Clamp(EndFraction(segment.To) * 100d, 0d, 100d);

        // An open end runs to the edge and is drawn dotted there, not cut.
        var cutLeft = !segment.OpenStart && segment.From.DayNumber < Start.DayNumber;
        var cutRight = !segment.OpenEnd && segment.To.DayNumber > End.DayNumber;

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
            string.IsNullOrEmpty(segment.Text) ? null : "cn-gantt-seg--text",
            place.CutLeft ? "cn-gantt-seg--cut-l" : null,
            place.CutRight ? "cn-gantt-seg--cut-r" : null,
            segment.OpenStart ? "cn-gantt-seg--open-l" : null,
            segment.OpenEnd ? "cn-gantt-seg--open-r" : null,
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
        CnTimelineSegmentTone.Warn => "cn-gantt-seg--warn",
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
