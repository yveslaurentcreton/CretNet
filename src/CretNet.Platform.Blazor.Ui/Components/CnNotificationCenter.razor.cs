using CretNet.Platform.Blazor.Ui.Notifications;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnNotificationCenter : IDisposable
{
    [Inject] public CnNotificationState State { get; set; } = default!;

    /// <summary>Raised when a row with an action path is opened. The host
    /// navigates — the component does not know what a route is.</summary>
    [Parameter] public EventCallback<CnNotificationItem> OnOpen { get; set; }

    [Parameter] public EventCallback OnClose { get; set; }

    // Resource-backed defaults; hosts may override wording with their own resources.
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

    /// <summary>Empty-state line per filter — "nothing archived" and "all
    /// caught up" are different kinds of empty.</summary>
    [Parameter] public Func<CnNotificationFilter, string> EmptyLabel { get; set; } = filter => filter switch
    {
        CnNotificationFilter.Archived => CnLabels.NothingArchived,
        CnNotificationFilter.ActionRequired => CnLabels.NoOpenActions,
        _ => CnLabels.YouAreAllCaughtUp,
    };

    /// <summary>How a moment is written. Default is a relative phrase: an
    /// inbox answers "how long has this been sitting here", not "what time
    /// was it".</summary>
    [Parameter] public Func<DateTimeOffset, string> FormatWhen { get; set; } = occurredAt =>
    {
        var elapsed = DateTimeOffset.UtcNow - occurredAt;
        if (elapsed < TimeSpan.FromMinutes(1)) return CnLabels.JustNow;
        if (elapsed < TimeSpan.FromHours(1)) return CnLabels.Format(CnLabels.MinutesAgo, (int)elapsed.TotalMinutes);
        if (elapsed < TimeSpan.FromDays(1)) return CnLabels.Format(CnLabels.HoursAgo, (int)elapsed.TotalHours);
        var days = (int)elapsed.TotalDays;
        return days == 1 ? CnLabels.Yesterday : CnLabels.Format(CnLabels.DaysAgo, days);
    };

    protected override async Task OnInitializedAsync()
    {
        State.Changed += OnChanged;
        await State.RefreshAsync(CnNotificationFilter.All);
    }

    private void OnChanged() => _ = InvokeAsync(StateHasChanged);

    private List<CnNotificationItem> Unread => State.Items.Where(item => item.ReadAt is null).ToList();
    private List<CnNotificationItem> Earlier => State.Items.Where(item => item.ReadAt is not null).ToList();

    private IReadOnlyList<(CnNotificationFilter Filter, string Label, int Count)> Tabs =>
    [
        (CnNotificationFilter.All, AllLabel, 0),
        (CnNotificationFilter.Unread, UnreadLabel, State.UnreadCount),
        (CnNotificationFilter.ActionRequired, ActionLabel, State.ActionRequiredCount),
        (CnNotificationFilter.Archived, ArchivedLabel, 0),
    ];

    private async Task OpenAsync(CnNotificationItem item)
    {
        if (item.ReadAt is null)
            await State.SetReadAsync(item, true);

        if (item.ActionPath is not null && OnOpen.HasDelegate)
            await OnOpen.InvokeAsync(item);
    }

    private static string Tone(CnNotificationSeverity severity) => severity switch
    {
        CnNotificationSeverity.Success => "success",
        CnNotificationSeverity.Warning => "warning",
        CnNotificationSeverity.Error => "error",
        _ => "info",
    };

    private static CnIconKind Icon(CnNotificationSeverity severity) => severity switch
    {
        CnNotificationSeverity.Success => CnIconKind.CheckCircle,
        CnNotificationSeverity.Warning => CnIconKind.Warning,
        CnNotificationSeverity.Error => CnIconKind.ErrorCircle,
        _ => CnIconKind.InfoCircle,
    };

    public void Dispose() => State.Changed -= OnChanged;
}
