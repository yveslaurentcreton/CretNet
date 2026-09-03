using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Drag-to-reorder that feels like holding the thing: the lifted row follows
/// the pointer and the other rows slide out of its way. One instance per
/// list; the drop arrives as "from index, to index" and the host decides
/// what to persist.
/// </summary>
/// <remarks>
/// <para>
/// This is the pointer-driven successor of <see cref="CnReorder{T}"/>. The
/// HTML5 drag events that one is built on give the browser's ghost image and
/// a drop line; they cannot move the row itself or animate the others. The
/// gesture lives in <c>cn-sortable.js</c>; nothing here touches the DOM.
/// </para>
/// <para>
/// Items are the container's direct children matching the item selector,
/// the gesture starts on the handle selector inside one of them. Nested
/// lists each get their own instance.
/// </para>
/// </remarks>
public sealed class CnSortable : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/cn-sortable.js";

    private readonly Func<int, int, Task> _onMoved;
    private readonly ElementReference _container;
    private IJSObjectReference? _module;
    private DotNetObjectReference<CnSortable>? _self;

    private CnSortable(ElementReference container, Func<int, int, Task> onMoved)
    {
        _container = container;
        _onMoved = onMoved;
    }

    /// <summary>
    /// Wires the gesture to a rendered container. Call from
    /// <c>OnAfterRenderAsync</c>, once per container element; dispose when
    /// the element goes.
    /// </summary>
    public static async Task<CnSortable> AttachAsync(
        IJSRuntime jsRuntime,
        ElementReference container,
        Func<int, int, Task> onMoved,
        string itemSelector = ".cn-sort-item",
        string handleSelector = ".cn-drag-handle")
    {
        var sortable = new CnSortable(container, onMoved);
        sortable._module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        sortable._self = DotNetObjectReference.Create(sortable);
        await sortable._module.InvokeVoidAsync("attach", container, sortable._self, itemSelector, handleSelector);
        return sortable;
    }

    [JSInvokable]
    public Task OnSortableMoved(int from, int to) => _onMoved(from, to);

    private Func<string, string, string?, Task>? _onDropped;
    private bool _isBoard;

    /// <summary>
    /// A board: columns a card may cross between. The drop arrives as
    /// "this card, into that column, after that card" — ids and keys are
    /// the <c>data-sort-id</c> of cards and the <c>data-sort-key</c> of
    /// columns, so the host decides what they mean.
    /// </summary>
    public static async Task<CnSortable> AttachBoardAsync(
        IJSRuntime jsRuntime,
        ElementReference root,
        Func<string, string, string?, Task> onDropped,
        string columnSelector,
        string itemSelector = ".cn-sort-item")
    {
        var sortable = new CnSortable(root, (_, _) => Task.CompletedTask)
        {
            _onDropped = onDropped,
            _isBoard = true,
        };
        sortable._module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        sortable._self = DotNetObjectReference.Create(sortable);
        await sortable._module.InvokeVoidAsync("attachBoard", root, sortable._self, columnSelector, itemSelector);
        return sortable;
    }

    [JSInvokable]
    public Task OnSortableDropped(string itemId, string columnKey, string? afterItemId) =>
        _onDropped?.Invoke(itemId, columnKey, afterItemId) ?? Task.CompletedTask;

    /// <summary>
    /// The list as it reads after a drop: the item at <paramref name="from"/>
    /// taken out and put back at <paramref name="to"/>, counted in the list
    /// without it. Out-of-range indexes leave the list as it was.
    /// </summary>
    public static List<T> Move<T>(IReadOnlyList<T> items, int from, int to)
    {
        var list = items.ToList();
        if (from < 0 || from >= list.Count)
            return list;

        var item = list[from];
        list.RemoveAt(from);
        list.Insert(Math.Clamp(to, 0, list.Count), item);
        return list;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync(_isBoard ? "detachBoard" : "detach", _container);
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
            // The container may already be gone; nothing left to detach.
        }

        _self?.Dispose();
    }
}
