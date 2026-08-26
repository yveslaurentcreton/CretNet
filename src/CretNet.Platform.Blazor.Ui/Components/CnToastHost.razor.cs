using CretNet.Platform.Blazor.Ui.Toasts;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnToastHost : IDisposable
{
    [Inject] public CnToastService Toasts { get; set; } = default!;

    [Parameter] public CnToastPosition Position { get; set; } = CnToastPosition.BottomRight;

    /// <summary>How many toasts are on screen at once. The rest queue.</summary>
    [Parameter] public int MaxVisible { get; set; } = 3;

    // Chrome strings as parameters with English defaults — the RCL carries no
    // resource dependency; hosts localise by passing their own labels.
    [Parameter] public string AriaLabel { get; set; } = "Notifications";
    [Parameter] public string DismissLabel { get; set; } = "Dismiss";

    /// <summary>Label for the overflow pill, given the number queued behind
    /// the cap. Default reads "+2 waiting · dismiss all".</summary>
    [Parameter] public Func<int, string> OverflowLabel { get; set; } =
        count => $"+{count} waiting · dismiss all";

    private readonly Dictionary<Guid, Lifetime> _lifetimes = [];
    private List<CnToastItem> _visible = [];
    private int _hidden;

    /// <summary>The countdown for one toast. It starts when the toast first
    /// becomes visible, not when it was raised.</summary>
    private sealed class Lifetime
    {
        public CancellationTokenSource? Cancellation;
        public TimeSpan Remaining;
        public bool Running;
    }

    protected override void OnInitialized() => Toasts.Changed += OnChanged;

    protected override void OnParametersSet() => Sync();

    private void OnChanged()
    {
        Sync();
        _ = InvokeAsync(StateHasChanged);
    }

    private void Sync()
    {
        var all = Toasts.Items;
        _visible = all.Take(MaxVisible).ToList();
        _hidden = Math.Max(0, all.Count - MaxVisible);

        // Forget the toasts that are gone, and start the ones that just
        // reached the front of the queue.
        foreach (var id in _lifetimes.Keys.Where(id => all.All(item => item.Id != id)).ToList())
        {
            _lifetimes[id].Cancellation?.Cancel();
            _lifetimes.Remove(id);
        }

        foreach (var item in _visible)
        {
            if (_lifetimes.ContainsKey(item.Id))
                continue;

            var lifetime = new Lifetime { Remaining = item.Duration };
            _lifetimes[item.Id] = lifetime;
            Start(item.Id, lifetime);
        }
    }

    private void Start(Guid id, Lifetime lifetime)
    {
        lifetime.Cancellation?.Cancel();
        lifetime.Cancellation = new CancellationTokenSource();
        lifetime.Running = true;

        var token = lifetime.Cancellation.Token;
        var startedAt = DateTimeOffset.UtcNow;
        var planned = lifetime.Remaining;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(planned, token);
                await InvokeAsync(() => Toasts.Dismiss(id));
            }
            catch (TaskCanceledException)
            {
                // Paused or dismissed by hand; keep what is left.
                lifetime.Remaining = planned - (DateTimeOffset.UtcNow - startedAt);
                if (lifetime.Remaining < TimeSpan.Zero)
                    lifetime.Remaining = TimeSpan.Zero;
            }
        }, token);
    }

    /// <summary>Reading a toast should not cost you the toast.</summary>
    private void Pause(Guid id)
    {
        if (!_lifetimes.TryGetValue(id, out var lifetime) || !lifetime.Running)
            return;

        lifetime.Running = false;
        lifetime.Cancellation?.Cancel();
    }

    private void Resume(Guid id)
    {
        if (!_lifetimes.TryGetValue(id, out var lifetime) || lifetime.Running)
            return;

        if (lifetime.Remaining <= TimeSpan.Zero)
        {
            Toasts.Dismiss(id);
            return;
        }

        Start(id, lifetime);
    }

    private async Task InvokeActionAsync(CnToastItem toast)
    {
        Toasts.Dismiss(toast.Id);
        if (toast.Action is { } action)
            await action();
    }

    private Task DismissAllAsync()
    {
        Toasts.DismissAll();
        return Task.CompletedTask;
    }

    private string LifeStyle(CnToastItem toast) =>
        $"animation-duration: {toast.Duration.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture)}ms";

    private string Anchor => Position switch
    {
        CnToastPosition.TopLeft => "topleft",
        CnToastPosition.TopCenter => "topcenter",
        CnToastPosition.TopRight => "topright",
        CnToastPosition.BottomLeft => "bottomleft",
        CnToastPosition.BottomCenter => "bottomcenter",
        _ => "bottomright",
    };

    private static string Tone(CnToastSeverity severity) => severity switch
    {
        CnToastSeverity.Success => "success",
        CnToastSeverity.Warning => "warning",
        CnToastSeverity.Error => "error",
        _ => "info",
    };

    private static CnIconKind Icon(CnToastSeverity severity) => severity switch
    {
        CnToastSeverity.Success => CnIconKind.CheckCircle,
        CnToastSeverity.Warning => CnIconKind.Warning,
        CnToastSeverity.Error => CnIconKind.ErrorCircle,
        _ => CnIconKind.InfoCircle,
    };

    public void Dispose()
    {
        Toasts.Changed -= OnChanged;
        foreach (var lifetime in _lifetimes.Values)
            lifetime.Cancellation?.Cancel();
    }
}
