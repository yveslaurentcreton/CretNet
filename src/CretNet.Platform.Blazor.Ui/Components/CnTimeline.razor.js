// CnTimeline scrolling through time. The widths are CSS: the track is the
// visible width times months / visible months, so a zoom is only a different
// --cn-gantt-visible. This module moves the scroll position and reports what
// is in view; it never renders. It:
//  - starts with today a third from the left of the visible track;
//  - pans by dragging with a mouse or pen (touch scrolls natively) and
//    swallows the click that ends a drag, so a segment is not opened by it;
//  - moves a month with the arrow keys, to either end with Home/End;
//  - keeps the day in the middle of the view in place when the track
//    changes width (a zoom, a resize);
//  - reports the first and last month in view and whether either end is
//    reached, only when one of them changes;
//  - marks the scroller data-more-l / data-more-r when time is hidden on that
//    side (attributes Blazor never renders, so a render never wipes them);
//  - remembers a zoom per key in localStorage, harmlessly when that fails.

const KEY_PREFIX = 'cn-timeline-zoom:';
const DRAG_THRESHOLD = 4;
const states = new WeakMap();

const smooth = () =>
    !(globalThis.matchMedia && globalThis.matchMedia('(prefers-reduced-motion: reduce)').matches);

/** The grid's geometry inside the scroller, or null when it is not there. */
export function measure(scroller) {
    const grid = scroller?.querySelector(':scope > .cn-gantt');
    const axis = grid?.querySelector('.cn-gantt-axis');
    if (!grid || !axis) {
        return null;
    }

    const gridRect = grid.getBoundingClientRect();
    const axisRect = axis.getBoundingClientRect();
    const trackWidth = axisRect.width;
    const nameWidth = axisRect.left - gridRect.left;
    const fixedRight = Math.max(0, gridRect.width - nameWidth - trackWidth);
    return {
        trackWidth,
        viewWidth: Math.max(1, scroller.clientWidth - nameWidth - fixedRight),
        months: Math.max(1, Number.parseInt(grid.dataset.months, 10) || 1),
        today: Number.parseFloat(grid.dataset.today),
        left: scroller.scrollLeft,
        max: Math.max(0, scroller.scrollWidth - scroller.clientWidth),
    };
}

/** First and last month (mostly) in view and whether an end is reached. Exported for tests. */
export function viewOf(m) {
    const monthWidth = m.trackWidth / m.months;
    const first = Math.min(m.months - 1, Math.max(0, Math.round(m.left / monthWidth)));
    const last = Math.min(m.months - 1, Math.max(first, Math.round((m.left + m.viewWidth) / monthWidth) - 1));
    return { first, last, atStart: m.left <= 1, atEnd: m.left >= m.max - 1 };
}

/** The scroll position that puts today a third from the left. Exported for tests. */
export function todayLeft(m) {
    const today = Number.isFinite(m.today) ? Math.min(1, Math.max(0, m.today)) : 0;
    return Math.min(m.max, Math.max(0, today * m.trackWidth - m.viewWidth / 3));
}

/** The scroll position that keeps a remembered middle in the middle. Exported for tests. */
export function centredLeft(m, centre) {
    return Math.min(m.max, Math.max(0, centre * m.trackWidth - m.viewWidth / 2));
}

function report(scroller, state) {
    const m = measure(scroller);
    if (!m) {
        return;
    }

    // A width change lands here before the resize observer: leave the
    // remembered middle alone until the observer has re-centred.
    if (m.trackWidth === state.trackWidth) {
        state.centre = (m.left + m.viewWidth / 2) / m.trackWidth;
    }

    const view = viewOf(m);
    scroller.toggleAttribute('data-more-l', !view.atStart);
    scroller.toggleAttribute('data-more-r', !view.atEnd);

    const last = state.view;
    if (last && last.first === view.first && last.last === view.last && last.atStart === view.atStart && last.atEnd === view.atEnd) {
        return;
    }

    state.view = view;
    state.dotNet?.invokeMethodAsync('OnViewChanged', view.first, view.last, view.atStart, view.atEnd).catch(() => { });
}

export function attach(scroller, dotNet) {
    if (!scroller) {
        return;
    }
    if (states.has(scroller)) {
        detach(scroller);
    }

    const state = { dotNet, view: null, centre: 0, trackWidth: 0, frame: 0, drag: null, swallowClick: false };
    states.set(scroller, state);

    const schedule = () => {
        if (state.frame) {
            return;
        }
        state.frame = requestAnimationFrame(() => {
            state.frame = 0;
            report(scroller, state);
        });
    };

    state.onScroll = schedule;

    state.onKeyDown = event => {
        const m = measure(scroller);
        if (!m) {
            return;
        }
        let left = null;
        switch (event.key) {
            case 'ArrowLeft': left = m.left - m.trackWidth / m.months; break;
            case 'ArrowRight': left = m.left + m.trackWidth / m.months; break;
            case 'Home': left = 0; break;
            case 'End': left = m.max; break;
        }
        if (left === null) {
            return;
        }
        event.preventDefault();
        scroller.scrollTo({ left, behavior: 'auto' });
    };

    state.onPointerDown = event => {
        if (event.pointerType === 'touch' || event.button !== 0) {
            return;
        }
        state.drag = { x: event.clientX, left: scroller.scrollLeft, moved: false };
    };

    state.onPointerMove = event => {
        const drag = state.drag;
        if (!drag) {
            return;
        }
        const dx = event.clientX - drag.x;
        if (!drag.moved && Math.abs(dx) > DRAG_THRESHOLD) {
            drag.moved = true;
            scroller.setAttribute('data-dragging', '');
        }
        if (drag.moved) {
            scroller.scrollLeft = drag.left - dx;
        }
    };

    state.onPointerUp = () => {
        const drag = state.drag;
        state.drag = null;
        if (!drag?.moved) {
            return;
        }
        scroller.removeAttribute('data-dragging');
        // The click that ends a drag is dispatched right after this event.
        state.swallowClick = true;
        setTimeout(() => { state.swallowClick = false; }, 0);
    };

    state.onClick = event => {
        if (!state.swallowClick) {
            return;
        }
        state.swallowClick = false;
        event.preventDefault();
        event.stopPropagation();
    };

    scroller.addEventListener('scroll', state.onScroll, { passive: true });
    scroller.addEventListener('keydown', state.onKeyDown);
    scroller.addEventListener('pointerdown', state.onPointerDown);
    scroller.addEventListener('click', state.onClick, true);
    window.addEventListener('pointermove', state.onPointerMove);
    window.addEventListener('pointerup', state.onPointerUp);
    window.addEventListener('pointercancel', state.onPointerUp);

    if (typeof ResizeObserver === 'function') {
        state.resize = new ResizeObserver(() => {
            const m = measure(scroller);
            if (!m) {
                return;
            }
            if (state.trackWidth && m.trackWidth !== state.trackWidth) {
                scroller.scrollLeft = centredLeft(m, state.centre);
            }
            state.trackWidth = m.trackWidth;
            report(scroller, state);
        });
        state.resize.observe(scroller);
        const grid = scroller.querySelector(':scope > .cn-gantt');
        if (grid) {
            state.resize.observe(grid);
        }
    }
}

/** Puts today a third from the left (or the start, without today). */
export function scrollToToday(scroller, animate) {
    const state = states.get(scroller);
    const m = measure(scroller);
    if (!m) {
        return;
    }
    const left = todayLeft(m);
    if (state) {
        state.trackWidth = m.trackWidth;
        state.centre = (left + m.viewWidth / 2) / m.trackWidth;
    }
    scroller.scrollTo({ left, behavior: animate && smooth() ? 'smooth' : 'auto' });
    if (state) {
        report(scroller, state);
    }
}

/** Half a view earlier (-1) or later (1). */
export function page(scroller, direction) {
    const m = measure(scroller);
    if (!m) {
        return;
    }
    scroller.scrollBy({ left: Math.sign(direction) * m.viewWidth / 2, behavior: smooth() ? 'smooth' : 'auto' });
}

export function detach(scroller) {
    const state = states.get(scroller);
    if (!state) {
        return;
    }
    states.delete(scroller);
    if (state.frame) {
        cancelAnimationFrame(state.frame);
    }
    state.resize?.disconnect();
    scroller.removeEventListener('scroll', state.onScroll);
    scroller.removeEventListener('keydown', state.onKeyDown);
    scroller.removeEventListener('pointerdown', state.onPointerDown);
    scroller.removeEventListener('click', state.onClick, true);
    window.removeEventListener('pointermove', state.onPointerMove);
    window.removeEventListener('pointerup', state.onPointerUp);
    window.removeEventListener('pointercancel', state.onPointerUp);
    state.dotNet = null;
}

/** The zoom remembered under a key, or null (nothing stored, or no storage). */
export function readZoom(key) {
    try {
        const value = globalThis.localStorage?.getItem(KEY_PREFIX + key);
        const months = value === null || value === undefined ? Number.NaN : Number.parseInt(value, 10);
        return Number.isFinite(months) ? months : null;
    } catch {
        return null;
    }
}

export function writeZoom(key, months) {
    try {
        globalThis.localStorage?.setItem(KEY_PREFIX + key, String(months));
    } catch {
        // A private window or blocked storage: the zoom is simply not remembered.
    }
}
