namespace CretNet.Platform.Blazor.Notifications;

/// <summary>Scoped event-driven state for the notification bell and inbox.</summary>
public sealed class CnpNotificationState : IDisposable
{
    private const int PageSize = 30;
    private readonly ICnpNotificationClient _client;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly CancellationTokenSource _disposeCancellation = new();

    public CnpNotificationState(ICnpNotificationClient client)
    {
        _client = client;
    }

    public event Action? Changed;

    public IReadOnlyList<CnpNotificationItem> Items { get; private set; } = [];
    public int UnreadCount { get; private set; }
    public int ActionRequiredCount { get; private set; }
    public CnpNotificationFilter Filter { get; private set; }
    public string? NextCursor { get; private set; }
    public bool IsLoading { get; private set; }
    public bool IsLoadingMore { get; private set; }
    public string? Error { get; private set; }

    public async Task RefreshSummaryAsync(CancellationToken cancellationToken = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _disposeCancellation.Token);
        var summary = await _client.GetSummaryAsync(linked.Token);
        UnreadCount = summary.UnreadCount;
        ActionRequiredCount = summary.ActionRequiredCount;
        Changed?.Invoke();
    }

    public async Task RefreshAsync(
        CnpNotificationFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            IsLoading = true;
            Error = null;
            if (filter.HasValue)
                Filter = filter.Value;
            Changed?.Invoke();

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposeCancellation.Token);
            var page = await _client.GetPageAsync(Filter, null, PageSize, linked.Token);
            var summary = await _client.GetSummaryAsync(linked.Token);
            Items = page.Items;
            NextCursor = page.NextCursor;
            UnreadCount = summary.UnreadCount;
            ActionRequiredCount = summary.ActionRequiredCount;
        }
        catch (OperationCanceledException) when (_disposeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            Error = exception.Message;
        }
        finally
        {
            IsLoading = false;
            Changed?.Invoke();
            _gate.Release();
        }
    }

    public async Task LoadMoreAsync(CancellationToken cancellationToken = default)
    {
        if (NextCursor is null || IsLoadingMore)
            return;

        IsLoadingMore = true;
        Changed?.Invoke();
        try
        {
            Error = null;
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposeCancellation.Token);
            var page = await _client.GetPageAsync(Filter, NextCursor, PageSize, linked.Token);
            Items = Items.Concat(page.Items).DistinctBy(item => item.Id).ToList();
            NextCursor = page.NextCursor;
        }
        catch (OperationCanceledException) when (_disposeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            Error = exception.Message;
        }
        finally
        {
            IsLoadingMore = false;
            Changed?.Invoke();
        }
    }

    public async Task SetReadAsync(CnpNotificationItem item, bool read, CancellationToken cancellationToken = default)
    {
        await ExecuteMutationAsync(async token =>
        {
            await _client.SetReadAsync(item.Id, read, token);
            DateTimeOffset? timestamp = read ? DateTimeOffset.UtcNow : null;
            Items = Items.Select(candidate => candidate.Id == item.Id
                ? candidate with { ReadAt = timestamp }
                : candidate).ToList();
        }, cancellationToken);
    }

    public async Task ArchiveAsync(CnpNotificationItem item, CancellationToken cancellationToken = default)
    {
        await ExecuteMutationAsync(async token =>
        {
            await _client.ArchiveAsync(item.Id, token);
            Items = Items.Where(candidate => candidate.Id != item.Id).ToList();
        }, cancellationToken);
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteMutationAsync(async token =>
        {
            await _client.MarkAllReadAsync(token);
            var now = DateTimeOffset.UtcNow;
            Items = Items.Select(item => item with { ReadAt = item.ReadAt ?? now }).ToList();
        }, cancellationToken);
    }

    private async Task ExecuteMutationAsync(
        Func<CancellationToken, Task> mutation,
        CancellationToken cancellationToken)
    {
        Error = null;
        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposeCancellation.Token);
            await mutation(linked.Token);
            await RefreshSummaryAsync(linked.Token);
        }
        catch (OperationCanceledException) when (_disposeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            Error = exception.Message;
        }
        finally
        {
            Changed?.Invoke();
        }
    }

    public void Dispose()
    {
        _disposeCancellation.Cancel();
        _disposeCancellation.Dispose();
        _gate.Dispose();
    }
}
