using CretNet.Platform.Blazor.Ui.Notifications;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnNotificationBell : IDisposable
{
    [Inject] public CnNotificationState State { get; set; } = default!;

    /// <summary>Raised when a row with an action path is opened; the host
    /// navigates.</summary>
    [Parameter] public EventCallback<CnNotificationItem> OnOpen { get; set; }

    /// <summary>How often the badge re-asks for the counts. Zero disables
    /// polling — for a host that pushes instead.</summary>
    [Parameter] public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(30);

    // Resource-backed chrome strings, with host overrides; passed straight
    // through to the panel so a host localises in one place.
    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    [Parameter] public string Title { get => field ?? CnLabels.Notifications; set; } = null!;
    [Parameter] public string MarkAllReadLabel { get => field ?? CnLabels.MarkAllRead; set; } = null!;
    [Parameter] public string CloseLabel { get => field ?? CnLabels.Close; set; } = null!;
    [Parameter] public string AllLabel { get => field ?? CnLabels.All; set; } = null!;
    [Parameter] public string UnreadLabel { get => field ?? CnLabels.Unread; set; } = null!;
    [Parameter] public string ActionLabel { get => field ?? CnLabels.Action; set; } = null!;
    [Parameter] public string ArchivedLabel { get => field ?? CnLabels.Archive; set; } = null!;
    [Parameter] public string NewGroupLabel { get => field ?? CnLabels.New; set; } = null!;
    [Parameter] public string EarlierGroupLabel { get => field ?? CnLabels.Earlier; set; } = null!;
    [Parameter] public string OlderLabel { get => field ?? CnLabels.OlderNotifications; set; } = null!;
    [Parameter] public string LoadingLabel { get => field ?? CnLabels.Ellipsis; set; } = null!;
    [Parameter] public string FailedLabel { get => field ?? CnLabels.CouldNotLoadNotifications; set; } = null!;
    [Parameter] public string RetryLabel { get => field ?? CnLabels.TryAgain; set; } = null!;
    [Parameter] public string ActionNeededLabel { get => field ?? CnLabels.ActionNeeded; set; } = null!;
    [Parameter] public string ArchiveLabel { get => field ?? CnLabels.Archive; set; } = null!;
    [Parameter] public string MarkReadLabel { get => field ?? CnLabels.MarkRead; set; } = null!;
    [Parameter] public string MarkUnreadLabel { get => field ?? CnLabels.MarkUnread; set; } = null!;
    #pragma warning restore BL0007

    [Parameter] public Func<CnNotificationFilter, string> EmptyLabel { get; set; } = filter => filter switch
    {
        CnNotificationFilter.Archived => CnLabels.NothingArchived,
        CnNotificationFilter.ActionRequired => CnLabels.NoOpenActions,
        _ => CnLabels.YouAreAllCaughtUp,
    };

    [Parameter] public Func<DateTimeOffset, string> FormatWhen { get; set; } = occurredAt =>
    {
        var elapsed = DateTimeOffset.UtcNow - occurredAt;
        if (elapsed < TimeSpan.FromMinutes(1)) return CnLabels.JustNow;
        if (elapsed < TimeSpan.FromHours(1)) return CnLabels.Format(CnLabels.MinutesAgo, (int)elapsed.TotalMinutes);
        if (elapsed < TimeSpan.FromDays(1)) return CnLabels.Format(CnLabels.HoursAgo, (int)elapsed.TotalHours);
        var days = (int)elapsed.TotalDays;
        return days == 1 ? CnLabels.Yesterday : CnLabels.Format(CnLabels.DaysAgo, days);
    };

    private bool _open;
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _polling;

    protected override void OnInitialized()
    {
        State.Changed += OnChanged;

        if (PollInterval <= TimeSpan.Zero)
            return;

        _polling = new CancellationTokenSource();
        _timer = new PeriodicTimer(PollInterval);
        _ = PollAsync(_polling.Token);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SafeRefreshAsync();
    }

    private async Task PollAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (_timer is not null && await _timer.WaitForNextTickAsync(cancellationToken))
                await SafeRefreshAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>A shell must not start nagging about connectivity: a failed
    /// count catches up on the next tick.</summary>
    private async Task SafeRefreshAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await State.RefreshSummaryAsync(cancellationToken);
        }
        catch
        {
        }
    }

    private async Task ToggleAsync()
    {
        _open = !_open;
        if (_open)
            await State.RefreshAsync(CnNotificationFilter.All);
    }

    private void Close() => _open = false;

    private async Task OpenAsync(CnNotificationItem item)
    {
        _open = false;
        if (OnOpen.HasDelegate)
            await OnOpen.InvokeAsync(item);
    }

    private void OnChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.Changed -= OnChanged;
        _polling?.Cancel();
        _polling?.Dispose();
        _timer?.Dispose();
    }
}
