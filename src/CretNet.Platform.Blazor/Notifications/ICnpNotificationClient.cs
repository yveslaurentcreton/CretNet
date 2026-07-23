namespace CretNet.Platform.Blazor.Notifications;

/// <summary>
/// Host-provided transport for the reusable notification inbox. CretNet owns
/// presentation and state, while the host owns persistence and authorization.
/// </summary>
public interface ICnpNotificationClient
{
    Task<CnpNotificationPage> GetPageAsync(
        CnpNotificationFilter filter,
        string? cursor,
        int take,
        CancellationToken cancellationToken);

    Task<CnpNotificationSummary> GetSummaryAsync(CancellationToken cancellationToken);
    Task SetReadAsync(Guid id, bool read, CancellationToken cancellationToken);
    Task ArchiveAsync(Guid id, CancellationToken cancellationToken);
    Task MarkAllReadAsync(CancellationToken cancellationToken);
}
