namespace CretNet.Platform.Blazor.Notifications;

public sealed record CnpNotificationItem(
    Guid Id,
    string Category,
    CnpNotificationSeverity Severity,
    string Type,
    string Title,
    string Message,
    string? ActionPath,
    string? SubjectType,
    Guid? SubjectId,
    bool RequiresAction,
    DateTimeOffset OccurredAt,
    DateTimeOffset? ReadAt,
    DateTimeOffset? ArchivedAt);

public sealed record CnpNotificationPage(
    IReadOnlyList<CnpNotificationItem> Items,
    string? NextCursor);

public sealed record CnpNotificationSummary(
    int UnreadCount,
    int ActionRequiredCount,
    DateTimeOffset ServerTime);

public enum CnpNotificationSeverity
{
    Information,
    Success,
    Warning,
    Error
}

public enum CnpNotificationFilter
{
    All,
    Unread,
    ActionRequired,
    Archived
}
