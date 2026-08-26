namespace CretNet.Platform.Blazor.Ui.Toasts;

/// <summary>
/// Raises toasts. <see cref="Components.CnToastHost"/> renders whatever is on
/// the stack and owns every timer, so a toast that is queued behind the
/// visible cap does not start expiring until it is on screen.
/// </summary>
public sealed class CnToastService
{
    private readonly List<CnToastItem> _items = [];

    public IReadOnlyList<CnToastItem> Items => _items;

    public event Action? Changed;

    /// <summary>How long a toast stays once visible, unless the caller says
    /// otherwise. Errors get longer: they are the ones worth reading.</summary>
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ErrorDuration { get; set; } = TimeSpan.FromSeconds(9);

    public CnToastItem Show(
        CnToastSeverity severity,
        string title,
        string? message = null,
        TimeSpan? duration = null,
        string? actionLabel = null,
        Func<Task>? action = null)
    {
        var item = new CnToastItem(
            Guid.NewGuid(),
            severity,
            title,
            message,
            duration ?? (severity == CnToastSeverity.Error ? ErrorDuration : DefaultDuration),
            actionLabel,
            action);

        _items.Add(item);
        Changed?.Invoke();
        return item;
    }

    public CnToastItem Information(string title, string? message = null) =>
        Show(CnToastSeverity.Information, title, message);

    public CnToastItem Success(string title, string? message = null) =>
        Show(CnToastSeverity.Success, title, message);

    public CnToastItem Warning(string title, string? message = null) =>
        Show(CnToastSeverity.Warning, title, message);

    public CnToastItem Error(string title, string? message = null) =>
        Show(CnToastSeverity.Error, title, message);

    public void Dismiss(Guid id)
    {
        if (_items.RemoveAll(item => item.Id == id) > 0)
            Changed?.Invoke();
    }

    public void DismissAll()
    {
        if (_items.Count == 0)
            return;

        _items.Clear();
        Changed?.Invoke();
    }
}
