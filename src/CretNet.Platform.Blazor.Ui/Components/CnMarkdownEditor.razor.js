// Collocated JS module for CnMarkdownEditor. .NET owns every text change
// (CnMarkdownTextCommands, CnHtmlToMarkdown); this module only reports the
// selection, intercepts shortcuts and HTML paste synchronously, writes edits
// back into the textarea and grows it with its content.
const instances = new WeakMap();

const SHORTCUTS = new Set(['b', 'i', 'k']);

export function init(textarea, dotnetRef) {
    if (!textarea || instances.has(textarea))
        return;

    const onKeyDown = (e) => {
        if (!(e.ctrlKey || e.metaKey) || e.altKey || e.shiftKey)
            return;

        const key = e.key.toLowerCase();
        if (!SHORTCUTS.has(key) || textarea.readOnly || textarea.disabled)
            return;

        // Cancel the browser's own Ctrl+B/I/K (bookmarks, search) before the
        // asynchronous round trip.
        e.preventDefault();
        dotnetRef.invokeMethodAsync('OnShortcutAsync', key, textarea.selectionStart, textarea.selectionEnd);
    };

    const onPaste = (e) => {
        const html = e.clipboardData?.getData('text/html');
        if (!html || textarea.readOnly || textarea.disabled)
            return; // Plain text pastes natively.

        e.preventDefault();
        const text = e.clipboardData.getData('text/plain');
        dotnetRef.invokeMethodAsync('OnPasteHtmlAsync', html, text, textarea.selectionStart, textarea.selectionEnd);
    };

    const onInput = () => autosize(textarea);

    // Text rewraps when the field gets narrower or wider (window, split
    // mode); only width changes matter, the height is ours.
    let width = 0;
    const observer = typeof ResizeObserver === 'function'
        ? new ResizeObserver(entries => {
            const next = Math.round(entries[0].contentRect.width);
            if (next !== width) {
                width = next;
                autosize(textarea);
            }
        })
        : null;
    observer?.observe(textarea);

    textarea.addEventListener('keydown', onKeyDown);
    textarea.addEventListener('paste', onPaste);
    textarea.addEventListener('input', onInput);
    instances.set(textarea, { onKeyDown, onPaste, onInput, observer });
    autosize(textarea);
}

export function getSelection(textarea) {
    if (!textarea)
        return null;
    return { start: textarea.selectionStart, end: textarea.selectionEnd };
}

// Replaces only the changed middle of the text through insertText, so Ctrl+Z
// still undoes a toolbar command; falls back to setRangeText. The input event
// this raises is stopped at the textarea: .NET already knows the new value.
export function apply(textarea, text, selectionStart, selectionEnd) {
    if (!textarea)
        return;

    const current = textarea.value;
    if (current !== text) {
        let prefix = 0;
        const max = Math.min(current.length, text.length);
        while (prefix < max && current[prefix] === text[prefix])
            prefix++;
        let suffix = 0;
        while (suffix < max - prefix && current[current.length - 1 - suffix] === text[text.length - 1 - suffix])
            suffix++;

        const swallow = (e) => e.stopImmediatePropagation();
        textarea.addEventListener('input', swallow, { capture: true });
        try {
            textarea.focus();
            textarea.setSelectionRange(prefix, current.length - suffix);
            const middle = text.slice(prefix, text.length - suffix);
            let inserted = false;
            try {
                inserted = middle.length > 0
                    ? document.execCommand('insertText', false, middle)
                    : document.execCommand('delete', false);
            } catch {
                inserted = false;
            }
            if (!inserted || textarea.value !== text)
                textarea.value = text;
        } finally {
            textarea.removeEventListener('input', swallow, { capture: true });
        }
    }

    textarea.focus();
    textarea.setSelectionRange(selectionStart, selectionEnd);
    autosize(textarea);
}

export function autosize(textarea) {
    if (!textarea || !textarea.isConnected)
        return;

    const scroll = document.scrollingElement?.scrollTop;
    // 0, not auto: an auto-height textarea stretches to its grid row (the
    // preview beside it) and would measure that instead of its own text.
    textarea.style.height = '0px';
    if (textarea.scrollHeight > 0)
        textarea.style.height = `${textarea.scrollHeight}px`;
    else
        textarea.style.height = '';
    // Growing the field must not make the page jump.
    if (scroll !== undefined && document.scrollingElement)
        document.scrollingElement.scrollTop = scroll;
}

export function focus(textarea) {
    textarea?.focus();
}

export function destroy(textarea) {
    const instance = textarea && instances.get(textarea);
    if (!instance)
        return;

    textarea.removeEventListener('keydown', instance.onKeyDown);
    textarea.removeEventListener('paste', instance.onPaste);
    textarea.removeEventListener('input', instance.onInput);
    instance.observer?.disconnect();
    instances.delete(textarea);
}
