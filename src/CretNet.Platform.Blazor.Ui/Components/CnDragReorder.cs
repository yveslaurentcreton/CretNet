namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Reusable, JS-free HTML5 drag-and-drop reorder helper for &lt;tr draggable="true"&gt; rows
/// (M-018/F2, mirrors the docflow-editors mock's drag-handle/drop-indicator). Wire it up as:
///
/// <code>
/// &lt;tr draggable="true"
///     class="cn-line-row @_dragReorder.RowClass(i)"
///     @ondragstart="() => _dragReorder.StartDrag(i)"
///     @ondragover="() => _dragReorder.DragOver(i)" @ondragover:preventDefault
///     @ondrop="() => _dragReorder.Drop(myList, i)" @ondrop:preventDefault
///     @ondragend="() => _dragReorder.Reset()"&gt;
/// </code>
///
/// Operates over any <see cref="IList{T}"/> — including DynamicData's IExtendedList&lt;T&gt;
/// inside SourceList&lt;T&gt;.Edit(...) — so the caller keeps full control over how/when the
/// reorder commits (e.g. a SourceList-backed view model can wrap Drop in its own Edit call).
/// </summary>
public class CnDragReorder<T>
{
    private int? _dragIndex;
    private int? _overIndex;

    public bool IsDragging => _dragIndex.HasValue;

    public void StartDrag(int index) => _dragIndex = index;

    public void DragOver(int index) => _overIndex = index;

    /// <summary>Moves the dragged item to <paramref name="targetIndex"/> within <paramref name="items"/> and resets drag state.</summary>
    public void Drop(IList<T> items, int targetIndex)
    {
        if (_dragIndex is not { } sourceIndex || sourceIndex == targetIndex || sourceIndex < 0 || sourceIndex >= items.Count)
        {
            Reset();
            return;
        }

        var item = items[sourceIndex];
        items.RemoveAt(sourceIndex);

        var insertAt = targetIndex > sourceIndex ? targetIndex - 1 : targetIndex;
        insertAt = Math.Clamp(insertAt, 0, items.Count);
        items.Insert(insertAt, item);

        Reset();
    }

    public void Reset()
    {
        _dragIndex = null;
        _overIndex = null;
    }

    /// <summary>CSS class for the row at <paramref name="index"/> while a drag is in progress: dims the dragged row, paints a drop-above border on the row currently hovered.</summary>
    public string RowClass(int index)
    {
        if (!IsDragging)
            return string.Empty;

        if (_dragIndex == index)
            return "cn-line-dragging";

        return _overIndex == index ? "cn-line-drop-above" : string.Empty;
    }
}
