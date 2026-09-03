// Drag-to-reorder that feels like holding the thing. Pointer events, not
// HTML5 drag: the lifted item follows the pointer, the others slide out of
// its way, and the drop is reported as "from index, to index". State and
// writes stay in .NET; this module only moves pixels and reports intent.
//
// Items are the container's direct children matching itemSelector; the
// gesture starts on handleSelector inside one of them. Nested sortables
// work because a handle only counts for the nearest item, and that item
// must be a direct child of this container.

export function attach(container, dotNetRef, itemSelector, handleSelector) {
    if (!container)
        return;
    if (container.__cnSort)
        detach(container);

    const state = { drag: null };
    const items = () => Array.from(container.children).filter(el => el.matches(itemSelector));

    state.onPointerDown = event => {
        if (event.button !== 0)
            return;
        const handle = event.target.closest(handleSelector);
        if (!handle || !container.contains(handle))
            return;
        const item = handle.closest(itemSelector);
        if (!item || item.parentElement !== container)
            return;

        const list = items();
        const index = list.indexOf(item);
        if (index < 0)
            return;

        const rects = list.map(el => el.getBoundingClientRect());
        const gap = list.length > 1 ? Math.max(0, rects[1].top - rects[0].bottom) : 0;
        state.drag = {
            item, index, target: index, list, rects,
            span: rects[index].height + gap,
            startY: event.clientY,
            pointerId: event.pointerId,
            moved: false,
        };
        try { handle.setPointerCapture(event.pointerId); } catch { }
        event.preventDefault();
    };

    state.onPointerMove = event => {
        if (!container.isConnected) {
            detach(container);
            return;
        }
        const drag = state.drag;
        if (!drag || event.pointerId !== drag.pointerId)
            return;

        const dy = event.clientY - drag.startY;
        if (!drag.moved) {
            if (Math.abs(dy) < 4)
                return;
            drag.moved = true;
            container.classList.add('cn-sort-active');
            drag.item.classList.add('cn-sort-lifting');
        }

        drag.item.style.transform = 'translateY(' + dy + 'px)';

        // Where the lifted item's centre sits among the others' resting
        // midpoints is where it would land: that is the target index in the
        // list without it.
        const centre = drag.rects[drag.index].top + drag.rects[drag.index].height / 2 + dy;
        let target = 0;
        drag.rects.forEach((rect, i) => {
            if (i !== drag.index && centre > rect.top + rect.height / 2)
                target++;
        });
        drag.target = target;

        drag.list.forEach((el, i) => {
            if (i === drag.index)
                return;
            let shift = 0;
            if (i < drag.index && i >= target)
                shift = drag.span;
            else if (i > drag.index && i <= target)
                shift = -drag.span;
            el.style.transform = shift ? 'translateY(' + shift + 'px)' : '';
        });
    };

    const finish = (event, commit) => {
        const drag = state.drag;
        if (!drag || event.pointerId !== drag.pointerId)
            return;
        state.drag = null;
        container.classList.remove('cn-sort-active');
        drag.item.classList.remove('cn-sort-lifting');
        for (const el of drag.list)
            el.style.transform = '';

        if (drag.moved && commit && drag.target !== drag.index)
            dotNetRef.invokeMethodAsync('OnSortableMoved', drag.index, drag.target);
    };

    state.onPointerUp = event => finish(event, true);
    state.onPointerCancel = event => finish(event, false);

    container.addEventListener('pointerdown', state.onPointerDown);
    window.addEventListener('pointermove', state.onPointerMove);
    window.addEventListener('pointerup', state.onPointerUp);
    window.addEventListener('pointercancel', state.onPointerCancel);
    container.__cnSort = state;
}

export function detach(container) {
    const state = container ? container.__cnSort : null;
    if (!state)
        return;
    container.removeEventListener('pointerdown', state.onPointerDown);
    window.removeEventListener('pointermove', state.onPointerMove);
    window.removeEventListener('pointerup', state.onPointerUp);
    window.removeEventListener('pointercancel', state.onPointerCancel);
    delete container.__cnSort;
}
