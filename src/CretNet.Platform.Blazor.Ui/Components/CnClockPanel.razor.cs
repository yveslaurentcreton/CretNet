using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnClockPanel : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnClockPanel.razor.js";

    /// <summary>Geometry of the face, in pixels. The dial is twice the radius.</summary>
    private const double Radius = 120;
    private const double OuterRing = 96;
    private const double InnerRing = 62;

    private enum Step { Hour, Minute, Second }

    private readonly record struct Mark(int Value, string Label, bool Inner);

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public TimeOnly Value { get; set; } = new(9, 0);
    [Parameter] public EventCallback<TimeOnly> ValueChanged { get; set; }

    [Parameter] public bool Seconds { get; set; }

    /// <summary>Raised once the last part has been picked.</summary>
    [Parameter] public EventCallback<TimeOnly> OnDone { get; set; }

    [Parameter] public RenderFragment? FooterActions { get; set; }

    [Parameter] public string HourHint { get; set; } = "Pick the hour";
    [Parameter] public string MinuteHint { get; set; } = "Pick the minutes";
    [Parameter] public string SecondHint { get; set; } = "Pick the seconds";

    private ElementReference _dialRef;
    private IJSObjectReference? _module;

    private Step _step = Step.Hour;
    private bool _dragging;

    // The dial's centre in viewport coordinates, read once per open: pointer
    // events carry client coordinates, and asking the browser for the rect on
    // every move would mean an interop call per pixel of drag.
    private double _centreX;
    private double _centreY;

    private Step[] Parts => Seconds ? [Step.Hour, Step.Minute, Step.Second] : [Step.Hour, Step.Minute];

    private int Current => _step switch
    {
        Step.Hour => Value.Hour,
        Step.Minute => Value.Minute,
        _ => Value.Second,
    };

    private string StepHint => _step switch
    {
        Step.Hour => HourHint,
        Step.Minute => MinuteHint,
        _ => SecondHint,
    };

    private string Display(Step part) => part switch
    {
        Step.Hour => Value.Hour.ToString("00", CultureInfo.InvariantCulture),
        Step.Minute => Value.Minute.ToString("00", CultureInfo.InvariantCulture),
        _ => Value.Second.ToString("00", CultureInfo.InvariantCulture),
    };

    /// <summary>The twelve labels of the current face; hours add an inner ring
    /// so the second half of the day fits.</summary>
    private IEnumerable<Mark> Marks
    {
        get
        {
            if (_step == Step.Hour)
            {
                for (var i = 0; i < 12; i++)
                    yield return new Mark(i == 0 ? 12 : i, (i == 0 ? 12 : i).ToString(CultureInfo.InvariantCulture), false);

                for (var i = 0; i < 12; i++)
                {
                    var value = i == 0 ? 0 : i + 12;
                    yield return new Mark(value, value.ToString("00", CultureInfo.InvariantCulture), true);
                }

                yield break;
            }

            for (var i = 0; i < 12; i++)
                yield return new Mark(i * 5, (i * 5).ToString("00", CultureInfo.InvariantCulture), false);
        }
    }

    /// <summary>Where a value sits on the face: twelve o'clock is up.</summary>
    private (double Angle, double RingRadius) Place(int value)
    {
        if (_step != Step.Hour)
            return (value % 60 * 6.0, OuterRing);

        var outer = value is >= 1 and <= 12;
        var index = outer ? value % 12 : value == 0 ? 0 : value - 12;
        return (index * 30.0, outer ? OuterRing : InnerRing);
    }

    private double HandRadius => Place(Current).RingRadius;

    private double HandAngle => Place(Current).Angle - 90;

    private double Left(Mark mark)
    {
        var (angle, ring) = Place(mark.Value);
        return Radius + Math.Cos((angle - 90) * Math.PI / 180) * ring;
    }

    private double Top(Mark mark)
    {
        var (angle, ring) = Place(mark.Value);
        return Radius + Math.Sin((angle - 90) * Math.PI / 180) * ring;
    }

    /// <summary>Restarts at the hour with a fresh value — called when the
    /// popover opens.</summary>
    public void Open(TimeOnly value)
    {
        Value = value;
        _step = Step.Hour;
        _dragging = false;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        try
        {
            _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
            var centre = await _module.InvokeAsync<double[]>("centreOf", _dialRef);
            if (centre.Length == 2)
            {
                _centreX = centre[0];
                _centreY = centre[1];
            }
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
            // Without the centre the dial cannot be aimed, but the field can
            // still be typed into — that is the primary path anyway.
        }
    }

    private async Task OnPointerDownAsync(PointerEventArgs args)
    {
        _dragging = true;
        await AimAsync(args, commit: false);
    }

    private async Task OnPointerMoveAsync(PointerEventArgs args)
    {
        if (_dragging)
            await AimAsync(args, commit: false);
    }

    private async Task OnPointerUpAsync(PointerEventArgs args)
    {
        if (!_dragging)
            return;

        _dragging = false;
        await AimAsync(args, commit: true);
    }

    /// <summary>Turns a pointer position into a value: the angle says which
    /// mark, and for hours the distance to the centre says which ring.</summary>
    private async Task AimAsync(PointerEventArgs args, bool commit)
    {
        var dx = args.ClientX - _centreX;
        var dy = args.ClientY - _centreY;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        var angle = Math.Atan2(dy, dx) * 180 / Math.PI + 90;
        if (angle < 0)
            angle += 360;

        if (_step == Step.Hour)
        {
            var index = (int)Math.Round(angle / 30) % 12;
            var inner = distance < (OuterRing + InnerRing) / 2;
            var hour = inner ? index == 0 ? 0 : index + 12 : index == 0 ? 12 : index;
            Value = new TimeOnly(hour, Value.Minute, Value.Second);
        }
        else
        {
            var unit = (int)Math.Round(angle / 6) % 60;
            Value = _step == Step.Minute
                ? new TimeOnly(Value.Hour, unit, Value.Second)
                : new TimeOnly(Value.Hour, Value.Minute, unit);
        }

        await ValueChanged.InvokeAsync(Value);

        if (!commit)
            return;

        var parts = Parts;
        var index2 = Array.IndexOf(parts, _step);
        if (index2 + 1 < parts.Length)
            _step = parts[index2 + 1];
        else
            await OnDone.InvokeAsync(Value);
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
