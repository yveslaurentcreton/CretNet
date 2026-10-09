// CnActionMenu placement. The list is absolutely positioned under its trigger
// by default, which any scrolling ancestor clips: a row kebab in a grid
// (overflow-x: auto also clips vertically), the lines of a document, a dialog
// body. While open the backdrop and the list are lifted into the browser's top
// layer (popover="manual"), so no overflow, z-index or stacking context of an
// ancestor can cut or cover them, and the list is pinned with fixed
// coordinates taken from the trigger: below it, or above it when there is more
// room up there; right-aligned to the trigger (left-aligned with AlignStart)
// and shifted to stay inside the viewport. It follows the trigger on scroll
// and resize. Without the Popover API it still escapes overflow as a plain
// fixed element. Blazor keeps the events: the elements stay where they are in
// the DOM, only their rendering moves.

const EDGE = 8;
const GAP = 4;
const MAX_HEIGHT = 420;
const open = new Map();

function lift(element) {
    if (!element || typeof element.showPopover !== 'function') {
        return false;
    }
    if (!element.hasAttribute('popover')) {
        element.setAttribute('popover', 'manual');
    }
    try {
        if (!element.matches(':popover-open')) {
            element.showPopover();
        }
        return true;
    } catch {
        // Not connected (the menu closed in the meantime) or not supported.
        return false;
    }
}

/** Positions the list against the trigger's rectangle. Exported for tests. */
export function position(list, anchor, alignStart) {
    if (!list || !anchor) {
        return;
    }

    const rect = anchor.getBoundingClientRect();
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    list.style.position = 'fixed';
    list.style.margin = '0';
    list.style.right = 'auto';
    list.style.bottom = 'auto';
    list.style.maxHeight = '';

    const spaceBelow = viewportHeight - rect.bottom - GAP - EDGE;
    const spaceAbove = rect.top - GAP - EDGE;
    const natural = Math.min(list.offsetHeight, MAX_HEIGHT);
    const flip = natural > spaceBelow && spaceAbove > spaceBelow;
    const room = Math.max(0, flip ? spaceAbove : spaceBelow);
    if (natural > room) {
        list.style.maxHeight = Math.max(120, room) + 'px';
    }
    const height = list.offsetHeight;

    const top = flip ? Math.max(EDGE, rect.top - GAP - height) : rect.bottom + GAP;
    list.style.top = top + 'px';

    const width = list.offsetWidth;
    const preferred = alignStart ? rect.left : rect.right - width;
    const left = Math.max(EDGE, Math.min(preferred, viewportWidth - width - EDGE));
    list.style.left = left + 'px';
    list.dataset.placement = flip ? 'above' : 'below';
}

/** Lifts and places an opened menu; safe to call again after a re-render. */
export function show(id, list, backdrop, anchor, alignStart) {
    if (!list || !anchor) {
        return;
    }

    // Backdrop first: the top layer stacks in show order, the list above it.
    lift(backdrop);
    lift(list);
    position(list, anchor, alignStart);

    if (open.has(id)) {
        open.get(id).list = list;
        return;
    }

    const entry = { list, anchor, alignStart, frame: 0 };
    entry.follow = () => {
        if (entry.frame) return;
        entry.frame = requestAnimationFrame(() => {
            entry.frame = 0;
            if (entry.list.isConnected && entry.anchor.isConnected) {
                position(entry.list, entry.anchor, entry.alignStart);
            }
        });
    };
    window.addEventListener('scroll', entry.follow, { capture: true, passive: true });
    window.addEventListener('resize', entry.follow, { passive: true });
    open.set(id, entry);
}

/** Stops following the trigger; the elements themselves leave with the render. */
export function hide(id) {
    const entry = open.get(id);
    if (!entry) {
        return;
    }
    window.removeEventListener('scroll', entry.follow, { capture: true });
    window.removeEventListener('resize', entry.follow);
    if (entry.frame) {
        cancelAnimationFrame(entry.frame);
    }
    open.delete(id);
}
