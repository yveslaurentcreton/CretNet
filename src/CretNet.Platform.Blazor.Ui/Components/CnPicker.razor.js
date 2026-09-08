const keyboardHandlers = new WeakMap();

// Cancel native defaults at the input before Blazor's delegated handler can
// close/rerender the popup. A render-based flag is both too late for fast keys
// and too broad: it also suppresses typing after an option was highlighted.
export function connect(input) {
    if (!input || keyboardHandlers.has(input)) return;
    const handler = event => {
        if (input.disabled || input.readOnly || event.isComposing) return;
        const open = input.getAttribute('aria-expanded') === 'true';
        if (event.key === 'ArrowDown' || event.key === 'ArrowUp' ||
            (open && (event.key === 'Enter' || event.key === 'Escape'))) {
            event.preventDefault();
        }
        // Keep bubbling: Blazor still owns selection and dismissal.
    };
    keyboardHandlers.set(input, handler);
    input.addEventListener('keydown', handler);
}

export function disconnect(input) {
    const handler = input && keyboardHandlers.get(input);
    if (!handler) return;
    input.removeEventListener('keydown', handler);
    keyboardHandlers.delete(input);
}

// CnPicker dropdown placement: the dropdown is absolutely positioned inside the
// field by default, which gets clipped by any overflow ancestor (dialog bodies
// use overflow-y: auto). While open we promote it to position: fixed at the
// field's viewport coordinates, flipping above the field when the space below
// is too tight. Re-invoked after every render while open, so it tracks result
// count changes; scroll/resize while open is rare enough to ignore.
export function place(dropdown, anchor) {
    if (!dropdown || !anchor) {
        return;
    }

    const rect = anchor.getBoundingClientRect();
    const viewportHeight = window.innerHeight;
    const spaceBelow = viewportHeight - rect.bottom - 8;
    const spaceAbove = rect.top - 8;
    const flip = spaceBelow < 220 && spaceAbove > spaceBelow;

    // Snap the list height to whole option rows so the last visible option is
    // never cut in half (owner feedback: "half gerenderde lijnen"). 32 = the
    // option row height incl. its gap; 14 = dropdown padding.
    const available = Math.min(320, Math.max(120, flip ? spaceAbove : spaceBelow));
    const rowHeight = 32;
    const chrome = 14;
    const rows = Math.max(3, Math.floor((available - chrome) / rowHeight));
    const maxHeight = rows * rowHeight + chrome;

    // S-256: rich rows (title + context + meta) need more room than a narrow
    // field offers. The dropdown sizes to its content — never narrower than
    // the field, capped so it stays on screen — and shifts left when the
    // field sits near the right viewport edge.
    dropdown.style.position = 'fixed';
    dropdown.style.left = rect.left + 'px';
    dropdown.style.minWidth = rect.width + 'px';
    dropdown.style.width = 'max-content';
    dropdown.style.maxWidth = Math.min(460, window.innerWidth - 16) + 'px';
    dropdown.style.maxHeight = maxHeight + 'px';
    dropdown.style.overflowY = 'auto';

    const overflowRight = rect.left + dropdown.getBoundingClientRect().width - (window.innerWidth - 8);
    if (overflowRight > 0) {
        dropdown.style.left = Math.max(8, rect.left - overflowRight) + 'px';
    }

    if (flip) {
        dropdown.style.top = 'auto';
        dropdown.style.bottom = (viewportHeight - rect.top + 4) + 'px';
    } else {
        dropdown.style.bottom = 'auto';
        dropdown.style.top = (rect.bottom + 4) + 'px';
    }
}
