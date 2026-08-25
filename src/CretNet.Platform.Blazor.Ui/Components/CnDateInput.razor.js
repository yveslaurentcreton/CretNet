// The browser half of the date mask. C# owns the rules (see CnDateMask) and
// hands over the finished text; this module only writes it into the field and
// keeps the caret where the user left it — neither of which Blazor can do,
// because the DOM value it patches has no notion of a caret.

export { place } from './CnPicker.razor.js';

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
