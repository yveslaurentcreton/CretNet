// The browser half of the date mask. C# owns the rules (see CnDateMask) and
// hands over the finished text; this module only writes it into the field and
// keeps the caret where the user left it — neither of which Blazor can do,
// because the DOM value it patches has no notion of a caret.

/** Lifts the calendar out of its overflow ancestors. Inside a dialog the
 *  popover is otherwise clipped by the scrolling body, so while it is open it
 *  is promoted to fixed coordinates taken from the field, flipping above when
 *  there is more room up there. The picker's own place() is tuned for option
 *  lists (capped height, scrollbar); a calendar wants neither. */
export function placePanel(panel, anchor) {
    if (!panel || !anchor) {
        return;
    }

    const field = anchor.getBoundingClientRect();
    const panelHeight = panel.offsetHeight;
    const spaceBelow = window.innerHeight - field.bottom - 8;
    const spaceAbove = field.top - 8;
    const flip = spaceBelow < panelHeight && spaceAbove > spaceBelow;

    panel.style.position = 'fixed';
    panel.style.top = flip ? '' : (field.bottom + 6) + 'px';
    panel.style.bottom = flip ? (window.innerHeight - field.top + 6) + 'px' : '';

    // Keep it on screen when the field sits near the right edge.
    const width = panel.offsetWidth;
    const left = Math.max(8, Math.min(field.left, window.innerWidth - width - 8));
    panel.style.left = left + 'px';
}

const digitsIn = text => (text.match(/\d/g) || []).length;

function caretAfterDigit(text, count) {
    if (count <= 0) {
        return 0;
    }
    let seen = 0;
    for (let i = 0; i < text.length; i++) {
        if (/\d/.test(text[i]) && ++seen === count) {
            return i + 1;
        }
    }
    return text.length;
}

/** Replaces the field's text with the masked version, putting the caret back
 *  behind the same digit it was behind before. */
export function applyMask(input, masked) {
    if (!input) {
        return;
    }
    const before = digitsIn(input.value.slice(0, input.selectionStart ?? input.value.length));
    input.value = masked;
    const caret = caretAfterDigit(masked, before);
    try { input.setSelectionRange(caret, caret); } catch { }
}

/** Writes text without touching the caret beyond parking it at the end —
 *  used when the value changes from outside (picking a day, a preset). */
export function setText(input, text) {
    if (!input) {
        return;
    }
    input.value = text;
    try { input.setSelectionRange(text.length, text.length); } catch { }
}

export function selectAll(input) {
    input?.select?.();
}

/** Digits that ran past the end of this field, so the caller can hand them to
 *  whatever comes next. */
export function overflow(input, max) {
    if (!input) {
        return '';
    }
    return (input.value.match(/\d/g) || []).join('').slice(max);
}

export function caretAtStart(input) {
    return !!input && (input.selectionStart ?? 0) === 0 && (input.selectionEnd ?? 0) === 0;
}
