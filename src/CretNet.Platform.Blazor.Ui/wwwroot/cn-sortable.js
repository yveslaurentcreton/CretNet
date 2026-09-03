// Drag-to-reorder that feels like holding the thing. Pointer events, not
// HTML5 drag: the lifted item comes out of the flow and follows the pointer,
// a dashed placeholder keeps its place and moves to wherever it would land,
// and the other items slide to make room. The drop is reported as "from
// index, to index"; state and writes stay in .NET, this module only moves
// pixels and reports intent.
//
// Items are the container's direct children matching itemSelector; the
// gesture starts on handleSelector inside one of them. Nested sortables
// work because a handle only counts for the nearest item, and that item
// must be a direct child of this container. Table rows work too: their
// cell widths are frozen before the row is lifted, so it keeps its shape
// once it no longer sits in the table.

export function attach(container, dotNetRef, itemSelector, handleSelector) {
    if (!container)
        return;
    if (container.__cnSort)
        detach(container);

    const state = { drag: null };
    const items = () => Array.from(container.children).filter(el => el.matches(itemSelector) && !el.classList.contains('cn-sort-placeholder'));

    const makePlaceholder = (item, rect) => {
        const placeholder = document.createElement(item.tagName);
        placeholder.className = 'cn-sort-placeholder';
        placeholder.style.height = rect.height + 'px';
        if (item.tagName === 'TR') {
            const cell = document.createElement('td');
            cell.colSpan = Math.max(1, item.children.length);
            placeholder.appendChild(cell);
        }
        return placeholder;
    };

    const lift = (item, rect) => {
        if (item.tagName === 'TR') {
            for (const cell of item.children)
                cell.style.width = cell.getBoundingClientRect().width + 'px';
            item.style.display = 'table';
            item.style.tableLayout = 'fixed';
        }
        item.style.position = 'fixed';
        item.style.left = rect.left + 'px';
        item.style.top = rect.top + 'px';
        item.style.width = rect.width + 'px';
        item.style.height = rect.height + 'px';
        item.style.margin = '0';
        item.style.zIndex = '20';
        item.style.pointerEvents = 'none';
        item.classList.add('cn-sort-lifting');
    };

    const settle = item => {
        if (item.tagName === 'TR') {
            for (const cell of item.children)
                cell.style.width = '';
            item.style.display = '';
            item.style.tableLayout = '';
        }
        for (const prop of ['position', 'left', 'top', 'width', 'height', 'margin', 'zIndex', 'pointerEvents', 'transform', 'transition'])
            item.style[prop] = '';
        item.classList.remove('cn-sort-lifting');
    };

    // FLIP: remember where the resting items were, move the placeholder,
    // then let each item glide from its old spot to its new one.
    const snapshot = list => new Map(list.map(el => [el, el.getBoundingClientRect()]));
    const glide = (before, list) => {
        for (const el of list) {
            const prev = before.get(el);
            if (!prev)
                continue;
            const now = el.getBoundingClientRect();
            const dy = prev.top - now.top;
            if (!dy)
                continue;
            el.style.transition = 'none';
            el.style.transform = 'translateY(' + dy + 'px)';
            requestAnimationFrame(() => {
                el.style.transition = '';
                el.style.transform = '';
            });
        }
    };

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

        const rect = item.getBoundingClientRect();
        state.drag = {
            item, index, rect,
            target: index,
            placeholder: null,
            startX: event.clientX,
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

        const dx = event.clientX - drag.startX;
        const dy = event.clientY - drag.startY;
        if (!drag.moved) {
            if (Math.abs(dy) < 4 && Math.abs(dx) < 4)
                return;
            drag.moved = true;
            container.classList.add('cn-sort-active');
            drag.placeholder = makePlaceholder(drag.item, drag.rect);
            drag.item.before(drag.placeholder);
            lift(drag.item, drag.rect);
        }

        drag.item.style.transform = 'translate(' + dx + 'px,' + dy + 'px) rotate(1deg)';

        // Where the lifted item's centre sits among the resting items is
        // where it would land. The placeholder goes there; the rest glide.
        const centre = drag.rect.top + drag.rect.height / 2 + dy;
        const resting = items().filter(el => el !== drag.item);
        let target = 0;
        for (const el of resting) {
            const r = el.getBoundingClientRect();
            if (centre > r.top + r.height / 2)
                target++;
        }
        if (target === drag.target)
            return;
        drag.target = target;

        const before = snapshot(resting);
        const after = resting[target] ?? null;
        container.insertBefore(drag.placeholder, after);
        glide(before, resting);
    };

    const finish = (event, commit) => {
        const drag = state.drag;
        if (!drag || event.pointerId !== drag.pointerId)
            return;
        state.drag = null;
        container.classList.remove('cn-sort-active');

        if (!drag.moved)
            return;

        const changed = commit && drag.target !== drag.index;
        // The host reorders and re-renders on this call; the frame after
        // that is when the DOM shows the new order, and the moment to put
        // the lifted item back in the flow without a jump.
        const done = changed
            ? dotNetRef.invokeMethodAsync('OnSortableMoved', drag.index, drag.target)
            : Promise.resolve();
        done.catch(() => { }).then(() => requestAnimationFrame(() => requestAnimationFrame(() => {
            drag.placeholder?.remove();
            settle(drag.item);
        })));
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
