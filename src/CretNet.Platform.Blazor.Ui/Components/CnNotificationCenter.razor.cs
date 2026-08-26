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

    // Chrome strings as parameters with English defaults — the RCL carries no
    // resource dependency; hosts localise by passing their own labels.
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

    /// <summary>Empty-state line per filter — "nothing archived" and "all
    /// caught up" are different kinds of empty.</summary>
    [Parameter] public Func<CnNotificationFilter, string> EmptyLabel { get; set; } = filter => filter switch
    {
        CnNotificationFilter.Archived => "Nothing archived",
        CnNotificationFilter.ActionRequired => "No open actions",
        _ => "You are all caught up",
    };

    /// <summary>How a moment is written. Default is a relative phrase: an
    /// inbox answers "how long has this been sitting here", not "what time
    /// was it".</summary>
    [Parameter] public Func<DateTimeOffset, string> FormatWhen { get; set; } = occurredAt =>
    {
        var elapsed = DateTimeOffset.UtcNow - occurredAt;
        if (elapsed < TimeSpan.FromMinutes(1)) return "just now";
        if (elapsed < TimeSpan.FromHours(1)) return $"{(int)elapsed.TotalMinutes} min ago";
        if (elapsed < TimeSpan.FromDays(1)) return $"{(int)elapsed.TotalHours} h ago";
        var days = (int)elapsed.TotalDays;
        return days == 1 ? "yesterday" : $"{days} days ago";
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
