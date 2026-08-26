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

    // Chrome strings as parameters with English defaults; passed straight
    // through to the panel so a host localises in one place.
    [Parameter] public string Title { get; set; } = "Notifications";
    [Parameter] public string MarkAllReadLabel { get; set; } = "Mark all read";
    [Parameter] public string CloseLabel { get; set; } = "Close";
    [Parameter] public string AllLabel { get; set; } = "All";
    [Parameter] public string UnreadLabel { get; set; } = "Unread";
    [Parameter] public string ActionLabel { get; set; } = "Action";
    [Parameter] public string ArchivedLabel { get; set; } = "Archive";
    [Parameter] public string NewGroupLabel { get; set; } = "New";
    [Parameter] public string EarlierGroupLabel { get; set; } = "Earlier";
    [Parameter] public string OlderLabel { get; set; } = "Older notifications";
    [Parameter] public string LoadingLabel { get; set; } = "…";
    [Parameter] public string FailedLabel { get; set; } = "Could not load notifications";
    [Parameter] public string RetryLabel { get; set; } = "Try again";
    [Parameter] public string ActionNeededLabel { get; set; } = "Action needed";
    [Parameter] public string ArchiveLabel { get; set; } = "Archive";
    [Parameter] public string MarkReadLabel { get; set; } = "Mark read";
    [Parameter] public string MarkUnreadLabel { get; set; } = "Mark unread";

    [Parameter] public Func<CnNotificationFilter, string> EmptyLabel { get; set; } = filter => filter switch
    {
        CnNotificationFilter.Archived => "Nothing archived",
        CnNotificationFilter.ActionRequired => "No open actions",
        _ => "You are all caught up",
    };

    [Parameter] public Func<DateTimeOffset, string> FormatWhen { get; set; } = occurredAt =>
    {
        var elapsed = DateTimeOffset.UtcNow - occurredAt;
        if (elapsed < TimeSpan.FromMinutes(1)) return "just now";
        if (elapsed < TimeSpan.FromHours(1)) return $"{(int)elapsed.TotalMinutes} min ago";
        if (elapsed < TimeSpan.FromDays(1)) return $"{(int)elapsed.TotalHours} h ago";
        var days = (int)elapsed.TotalDays;
        return days == 1 ? "yesterday" : $"{days} days ago";
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
