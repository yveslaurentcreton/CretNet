using CretNet.Platform.Blazor.Ui.Notifications;
using CretNet.Platform.Blazor.Ui.Resources;

namespace CretNet.Platform.Blazor.Ui.Sample;

/// <summary>In-memory transport for exercising the library sample; no external effects.</summary>
public sealed class SampleNotifications : ICnNotificationClient
{
    private readonly List<CnNotificationItem> _items =
    [
        new(Guid.NewGuid(), "CretNet", CnNotificationSeverity.Information, "sample",
            CnLabels.NewItem, CnLabels.ActionNeeded, "/", null, null, true,
            DateTimeOffset.UtcNow, null, null),
    ];

    public Task<CnNotificationPage> GetPageAsync(CnNotificationFilter filter, string? cursor, int take, CancellationToken cancellationToken) =>
        Task.FromResult(new CnNotificationPage(_items.Where(item => filter switch
        {
            CnNotificationFilter.Archived => item.ArchivedAt is not null,
            CnNotificationFilter.Unread => item.ArchivedAt is null && item.ReadAt is null,
            CnNotificationFilter.ActionRequired => item.ArchivedAt is null && item.RequiresAction,
            _ => item.ArchivedAt is null,
        }).Take(take).ToList(), null));

    public Task<CnNotificationSummary> GetSummaryAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new CnNotificationSummary(
            _items.Count(item => item.ArchivedAt is null && item.ReadAt is null),
            _items.Count(item => item.ArchivedAt is null && item.RequiresAction),
            DateTimeOffset.UtcNow));

    public Task SetReadAsync(Guid id, bool read, CancellationToken cancellationToken)
    {
        var index = _items.FindIndex(item => item.Id == id);
        if (index >= 0)
            _items[index] = _items[index] with { ReadAt = read ? DateTimeOffset.UtcNow : null };
        return Task.CompletedTask;
    }

    public Task ArchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var index = _items.FindIndex(item => item.Id == id);
        if (index >= 0)
            _items[index] = _items[index] with { ArchivedAt = DateTimeOffset.UtcNow };
        return Task.CompletedTask;
    }

    public Task MarkAllReadAsync(CancellationToken cancellationToken)
    {
        for (var index = 0; index < _items.Count; index++)
            if (_items[index].ArchivedAt is null)
                _items[index] = _items[index] with { ReadAt = DateTimeOffset.UtcNow };
        return Task.CompletedTask;
    }
}
