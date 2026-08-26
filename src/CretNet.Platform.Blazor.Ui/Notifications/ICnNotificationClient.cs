namespace CretNet.Platform.Blazor.Ui.Notifications;

/// <summary>
/// Host-provided transport for the inbox. CretNet owns presentation and
/// state; the host owns persistence and authorization — which is the only
/// place that can know whose notifications these are.
/// </summary>
public interface ICnNotificationClient
{
    Task<CnNotificationPage> GetPageAsync(
        CnNotificationFilter filter,
        string? cursor,
        int take,
        CancellationToken cancellationToken);

    Task<CnNotificationSummary> GetSummaryAsync(CancellationToken cancellationToken);

    Task SetReadAsync(Guid id, bool read, CancellationToken cancellationToken);

    Task ArchiveAsync(Guid id, CancellationToken cancellationToken);

    Task MarkAllReadAsync(CancellationToken cancellationToken);
}
