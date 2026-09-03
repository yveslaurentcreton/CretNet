namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// The state of one drag-to-reorder gesture over a list, and the order it
/// produces when dropped.
/// </summary>
/// <remarks>
/// <para>
/// Blazor's HTML5 drag events are enough to reorder a list; what every list
/// ends up copying is the bookkeeping around them — which item is in the
/// air, which row it hovers, what the list looks like after the drop. That
/// bookkeeping lives here, with no markup, so a row can bind
/// <c>@ondragstart</c>, <c>@ondragenter</c> and <c>@ondrop</c> to three
/// calls and style itself off <see cref="IsDragging"/> and
/// <see cref="IsDropTarget"/> (the <c>.cn-line-dragging</c> and
/// <c>.cn-line-drop-above</c> classes in cn-ui.css).
/// </para>
/// <para>
/// Pure and synchronous on purpose: the drop returns a new order and the
/// caller decides what to persist, which keeps it testable without a
/// renderer.
/// </para>
/// </remarks>
public sealed class CnReorder<T> where T : class
{
    public T? Dragging { get; private set; }
    public T? DropTarget { get; private set; }

    public bool IsActive => Dragging is not null;
    public bool IsDragging(T item) => ReferenceEquals(Dragging, item);
    public bool IsDropTarget(T item) => IsActive && !IsDragging(item) && ReferenceEquals(DropTarget, item);

    public void Start(T item)
    {
        Dragging = item;
        DropTarget = null;
    }

    public void Over(T item) => DropTarget = item;

    public void End()
    {
        Dragging = null;
        DropTarget = null;
    }

    /// <summary>
    /// The list as it should read after the drop: the dragged item moved to
    /// just before the row it was released on. Dropped on itself, or with
    /// nothing in the air, the list comes back unchanged — and
    /// <see cref="End"/> is called either way.
    /// </summary>
    public IReadOnlyList<T> Drop(IReadOnlyList<T> items)
    {
        var dragging = Dragging;
        var target = DropTarget;
        End();

        if (dragging is null || target is null || ReferenceEquals(dragging, target))
            return items;

        var list = items.Where(item => !ReferenceEquals(item, dragging)).ToList();
        var at = list.FindIndex(item => ReferenceEquals(item, target));
        if (at < 0)
            return items;

        list.Insert(at, dragging);
        return list;
    }

    /// <summary>
    /// Same as <see cref="Drop(IReadOnlyList{T})"/>, but the dragged item
    /// lands <em>after</em> the target — for a drop on the last row, where
    /// "before" cannot reach the end.
    /// </summary>
    public IReadOnlyList<T> DropAfter(IReadOnlyList<T> items)
    {
        var dragging = Dragging;
        var target = DropTarget;
        End();

        if (dragging is null || target is null || ReferenceEquals(dragging, target))
            return items;

        var list = items.Where(item => !ReferenceEquals(item, dragging)).ToList();
        var at = list.FindIndex(item => ReferenceEquals(item, target));
        if (at < 0)
            return items;

        list.Insert(at + 1, dragging);
        return list;
    }
}
