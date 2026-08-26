namespace CretNet.Platform.Blazor.Ui.Notifications;

/// <summary>
/// Event-driven state behind the bell and the inbox: one filter, one page,
/// one summary. Read and archive are applied locally first so the panel
/// answers instantly, then confirmed against the client — a marked-read row
/// that only reacts after a round trip feels broken.
/// </summary>
public sealed class CnNotificationState : IDisposable
{
    private const int PageSize = 30;

    private readonly ICnNotificationClient? _client;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly CancellationTokenSource _disposing = new();

    /// <param name="client">
    /// Optional on purpose. A shell that renders the bell before its host has
    /// a transport should show an empty inbox, not take the render tree with
    /// it — which is exactly what happened in WAM (BUG-014) when the state
    /// could not be constructed at first paint.
    /// </param>
    public CnNotificationState(ICnNotificationClient? client = null) => _client = client;

    public event Action? Changed;

    public IReadOnlyList<CnNotificationItem> Items { get; private set; } = [];
    public int UnreadCount { get; private set; }
    public int ActionRequiredCount { get; private set; }
    public CnNotificationFilter Filter { get; private set; }
    public string? NextCursor { get; private set; }
    public bool IsLoading { get; private set; }
    public bool IsLoadingMore { get; private set; }
    public string? Error { get; private set; }

    /// <summary>Just the counts, for the badge. Cheap enough to poll.</summary>
    public async Task RefreshSummaryAsync(CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        using var linked = Link(cancellationToken);
        var summary = await _client.GetSummaryAsync(linked.Token);
        UnreadCount = summary.UnreadCount;
        ActionRequiredCount = summary.ActionRequiredCount;
        Changed?.Invoke();
    }

    public async Task RefreshAsync(
        CnNotificationFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            IsLoading = true;
            Error = null;
            if (filter is { } value)
                Filter = value;
            Changed?.Invoke();

            using var linked = Link(cancellationToken);
            var page = await _client.GetPageAsync(Filter, null, PageSize, linked.Token);
            var summary = await _client.GetSummaryAsync(linked.Token);

            Items = page.Items;
            NextCursor = page.NextCursor;
            UnreadCount = summary.UnreadCount;
            ActionRequiredCount = summary.ActionRequiredCount;
        }
        catch (OperationCanceledException)
        {
            // Superseded, or the circuit is going away.
        }
        catch (Exception exception)
        {
            Error = exception.Message;
        }
        finally
        {
            IsLoading = false;
            _gate.Release();
            Changed?.Invoke();
        }
    }

    public async Task LoadMoreAsync(CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        if (NextCursor is null || IsLoadingMore)
            return;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            IsLoadingMore = true;
            Changed?.Invoke();

            using var linked = Link(cancellationToken);
            var page = await _client.GetPageAsync(Filter, NextCursor, PageSize, linked.Token);

            Items = [.. Items, .. page.Items];
            NextCursor = page.NextCursor;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Error = exception.Message;
        }
        finally
        {
            IsLoadingMore = false;
            _gate.Release();
            Changed?.Invoke();
        }
    }

    public async Task SetReadAsync(CnNotificationItem item, bool read, CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        Apply(item.Id, current => current with { ReadAt = read ? DateTimeOffset.UtcNow : null });
        UnreadCount = Math.Max(0, UnreadCount + (read ? -1 : 1));
        Changed?.Invoke();

        using var linked = Link(cancellationToken);
        await _client.SetReadAsync(item.Id, read, linked.Token);
    }

    public async Task ArchiveAsync(CnNotificationItem item, CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        Items = Items.Where(current => current.Id != item.Id).ToList();
        if (item.ReadAt is null)
            UnreadCount = Math.Max(0, UnreadCount - 1);
        if (item.RequiresAction)
            ActionRequiredCount = Math.Max(0, ActionRequiredCount - 1);
        Changed?.Invoke();

        using var linked = Link(cancellationToken);
        await _client.ArchiveAsync(item.Id, linked.Token);
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        if (_client is null)
            return;

        var now = DateTimeOffset.UtcNow;
        Items = Items.Select(item => item.ReadAt is null ? item with { ReadAt = now } : item).ToList();
        UnreadCount = 0;
        Changed?.Invoke();

        using var linked = Link(cancellationToken);
        await _client.MarkAllReadAsync(linked.Token);
    }

    private void Apply(Guid id, Func<CnNotificationItem, CnNotificationItem> change) =>
        Items = Items.Select(item => item.Id == id ? change(item) : item).ToList();

    private CancellationTokenSource Link(CancellationToken cancellationToken) =>
        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposing.Token);

    public void Dispose()
    {
        _disposing.Cancel();
        _disposing.Dispose();
        _gate.Dispose();
    }
}
