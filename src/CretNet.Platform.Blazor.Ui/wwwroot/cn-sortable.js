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

// A board: several columns, and a card may cross from one to another. The
// lifted card comes out of the flow and follows the pointer; a placeholder
// takes its place and moves to wherever it would land, in whichever column
// the pointer is over. The drop is reported as "this card, into that column,
// after that card" — the host knows what a column means.
//
// Columns are root.querySelectorAll(columnSelector), each with data-sort-key;
// cards are a column's direct children matching itemSelector, each with
// data-sort-id. A press on a control inside a card is not a drag.
export function attachBoard(root, dotNetRef, columnSelector, itemSelector) {
    if (!root)
        return;
    if (root.__cnSortBoard)
        detachBoard(root);

    const state = { drag: null };
    const columns = () => Array.from(root.querySelectorAll(columnSelector));
    const cardsOf = column => Array.from(column.children).filter(el => el.matches(itemSelector) && !el.classList.contains('cn-sort-placeholder'));

    const snapshot = list => new Map(list.map(el => [el, el.getBoundingClientRect()]));
    const glide = (before, list) => {
        for (const el of list) {
            const prev = before.get(el);
            if (!prev)
                continue;
            const now = el.getBoundingClientRect();
            const dx = prev.left - now.left;
            const dy = prev.top - now.top;
            if (!dx && !dy)
                continue;
            el.style.transition = 'none';
            el.style.transform = 'translate(' + dx + 'px,' + dy + 'px)';
            requestAnimationFrame(() => {
                el.style.transition = '';
                el.style.transform = '';
            });
        }
    };

    state.onPointerDown = event => {
        if (event.button !== 0)
            return;
        if (event.target.closest('button, a, input, select, textarea, [data-no-drag]'))
            return;
        const item = event.target.closest(itemSelector);
        if (!item || !root.contains(item))
            return;
        const column = item.parentElement;
        if (!column || !column.matches(columnSelector))
            return;

        const rect = item.getBoundingClientRect();
        state.drag = {
            item, column, rect,
            placeholder: null,
            cache: null,
            lastColumn: null,
            lastIndex: -1,
            startX: event.clientX,
            startY: event.clientY,
            pointerId: event.pointerId,
            moved: false,
        };
        try { item.setPointerCapture(event.pointerId); } catch { }
        event.preventDefault();
    };

    state.onPointerMove = event => {
        if (!root.isConnected) {
            detachBoard(root);
            return;
        }
        const drag = state.drag;
        if (!drag || event.pointerId !== drag.pointerId)
            return;

        const dx = event.clientX - drag.startX;
        const dy = event.clientY - drag.startY;
        if (!drag.moved) {
            if (Math.hypot(dx, dy) < 5)
                return;
            drag.moved = true;
            root.classList.add('cn-sort-active');
            drag.placeholder = document.createElement(drag.item.tagName);
            drag.placeholder.className = 'cn-sort-placeholder';
            drag.placeholder.style.height = drag.rect.height + 'px';
            drag.item.before(drag.placeholder);
            drag.item.style.position = 'fixed';
            drag.item.style.left = drag.rect.left + 'px';
            drag.item.style.top = drag.rect.top + 'px';
            drag.item.style.width = drag.rect.width + 'px';
            drag.item.style.margin = '0';
            drag.item.style.zIndex = '20';
            drag.item.style.pointerEvents = 'none';
            drag.item.classList.add('cn-sort-lifting');
        }

        drag.item.style.transform = 'translate(' + dx + 'px,' + dy + 'px) rotate(1.5deg)';

        // The column under the pointer: elementFromPoint sees through the
        // lifted card because it takes no pointer events. Over the gap
        // between columns nothing changes.
        const under = document.elementFromPoint(event.clientX, event.clientY);
        const column = under ? under.closest(columnSelector) : null;
        if (!column || !root.contains(column))
            return;

        // Card midpoints are measured once per column visit and again after
        // the placeholder moved — not on every pointer event. A column can
        // hold a couple of hundred cards, and measuring them all sixty times
        // a second is the hiccup that showed on crossing into a column.
        if (!drag.cache || drag.cache.column !== column) {
            drag.cache = measure(column, drag.item);
            columns().forEach(c => c.classList.toggle('cn-sort-over', c === column));
        }

        const cards = drag.cache.cards;
        let index = cards.findIndex(c => event.clientY < c.mid);
        if (index < 0)
            index = cards.length;
        if (column === drag.lastColumn && index === drag.lastIndex)
            return;

        // Only the cards that actually shift glide: those from the
        // placeholder's old spot onward and from its new spot onward.
        const from = drag.placeholder.parentElement;
        const moving = [];
        if (from && from !== column)
            moving.push(...cardsFrom(from, drag.placeholder, drag.item));
        moving.push(...cards.slice(Math.min(index, drag.lastColumn === column ? drag.lastIndex : index)).map(c => c.el));

        const snap = snapshot(moving);
        column.insertBefore(drag.placeholder, cards[index]?.el ?? null);
        glide(snap, moving);

        drag.lastColumn = column;
        drag.lastIndex = index;
        drag.cache = measure(column, drag.item);
    };

    const measure = (column, lifted) => ({
        column,
        cards: cardsOf(column).filter(el => el !== lifted).map(el => {
            const r = el.getBoundingClientRect();
            return { el, mid: r.top + r.height / 2 };
        }),
    });

    // The cards of a column from the placeholder onward — the ones that
    // slide up when it leaves.
    const cardsFrom = (column, placeholder, lifted) => {
        const result = [];
        let seen = false;
        for (const el of column.children) {
            if (el === placeholder) { seen = true; continue; }
            if (seen && el !== lifted && el.matches(itemSelector))
                result.push(el);
        }
        return result;
    };

    const finish = (event, commit) => {
        const drag = state.drag;
        if (!drag || event.pointerId !== drag.pointerId)
            return;
        state.drag = null;
        root.classList.remove('cn-sort-active');
        columns().forEach(c => c.classList.remove('cn-sort-over'));
        if (!drag.moved)
            return;

        const placeholder = drag.placeholder;
        const column = placeholder.parentElement;
        let done = Promise.resolve();
        if (commit && column) {
            let after = placeholder.previousElementSibling;
            while (after && !after.matches(itemSelector))
                after = after.previousElementSibling;
            const afterId = after && after !== drag.item ? after.dataset.sortId : null;
            const changed = column !== drag.column || afterId !== previousOf(drag.item, itemSelector);
            if (changed)
                done = dotNetRef.invokeMethodAsync('OnSortableDropped', drag.item.dataset.sortId, column.dataset.sortKey, afterId ?? null);
        }
        done.catch(() => { }).then(() => requestAnimationFrame(() => requestAnimationFrame(() => {
            placeholder.remove();
            for (const prop of ['position', 'left', 'top', 'width', 'margin', 'zIndex', 'pointerEvents', 'transform', 'transition'])
                drag.item.style[prop] = '';
            drag.item.classList.remove('cn-sort-lifting');
        })));
    };

    const previousOf = (item, selector) => {
        let prev = item.previousElementSibling;
        while (prev && (!prev.matches(selector) || prev.classList.contains('cn-sort-placeholder')))
            prev = prev.previousElementSibling;
        return prev ? prev.dataset.sortId : null;
    };

    state.onPointerUp = event => finish(event, true);
    state.onPointerCancel = event => finish(event, false);

    root.addEventListener('pointerdown', state.onPointerDown);
    window.addEventListener('pointermove', state.onPointerMove);
    window.addEventListener('pointerup', state.onPointerUp);
    window.addEventListener('pointercancel', state.onPointerCancel);
    root.__cnSortBoard = state;
}

export function detachBoard(root) {
    const state = root ? root.__cnSortBoard : null;
    if (!state)
        return;
    root.removeEventListener('pointerdown', state.onPointerDown);
    window.removeEventListener('pointermove', state.onPointerMove);
    window.removeEventListener('pointerup', state.onPointerUp);
    window.removeEventListener('pointercancel', state.onPointerCancel);
    delete root.__cnSortBoard;
}
