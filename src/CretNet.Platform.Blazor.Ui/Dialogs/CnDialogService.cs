using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Dialogs;

/// <summary>
/// One open dialog on the <see cref="CnDialogService"/> stack. The content
/// component receives this instance as a cascading parameter and finishes the
/// dialog through <see cref="Close"/> / <see cref="Cancel"/>.
/// </summary>
public sealed class CnDialogInstance
{
    public required Type ContentType { get; init; }
    public required string Title { get; init; }
    public string? Width { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = [];

    internal TaskCompletionSource<object?> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Close(object? result) => Completion.TrySetResult(result);
    public void Cancel() => Completion.TrySetResult(null);
}

/// <summary>
/// Own dialog service (ADR-011, S-075): replaces FluentUI's IDialogService for
/// the Cn screens. <see cref="CnDialogHost"/> (in the main layout) renders the
/// stack; ShowAsync completes when the content closes or cancels.
/// </summary>
public class CnDialogService
{
    private readonly List<CnDialogInstance> _stack = [];

    public IReadOnlyList<CnDialogInstance> Stack => _stack;

    public event Action? Changed;

    public async Task<TResult?> ShowAsync<TContent, TResult>(string title, Dictionary<string, object>? parameters = null, string? width = null)
        where TContent : IComponent
    {
        var instance = new CnDialogInstance
        {
            ContentType = typeof(TContent),
            Title = title,
            Width = width,
            Parameters = parameters ?? [],
        };

        _stack.Add(instance);
        Changed?.Invoke();

        try
        {
            var result = await instance.Completion.Task;
            return result is TResult typed ? typed : default;
        }
        finally
        {
            _stack.Remove(instance);
            Changed?.Invoke();
        }
    }
}
