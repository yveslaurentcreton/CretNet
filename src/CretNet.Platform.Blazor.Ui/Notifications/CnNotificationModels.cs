namespace CretNet.Platform.Blazor.Ui.Notifications;

/// <summary>
/// One row in the inbox.
/// </summary>
/// <param name="ActionPath">Where opening the row takes you. Null means the
/// notification is informational and clicking it only marks it read.</param>
/// <param name="RequiresAction">Something is still expected of the reader.
/// This is what the "Action" filter counts, and it is not the same as unread:
/// you can have read a failure and still not have fixed it.</param>
public sealed record CnNotificationItem(
    Guid Id,
    string Category,
    CnNotificationSeverity Severity,
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

public sealed record CnNotificationPage(
    IReadOnlyList<CnNotificationItem> Items,
    string? NextCursor);

public sealed record CnNotificationSummary(
    int UnreadCount,
    int ActionRequiredCount,
    DateTimeOffset ServerTime);

public enum CnNotificationSeverity
{
    Information,
    Success,
    Warning,
    Error,
}

public enum CnNotificationFilter
{
    All,
    Unread,
    ActionRequired,
    Archived,
}
