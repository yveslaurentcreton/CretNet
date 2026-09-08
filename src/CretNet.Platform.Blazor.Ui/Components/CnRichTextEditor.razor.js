// Collocated JS module for CnRichTextEditor: owns the contenteditable behavior,
// toolbar commands and active-state highlighting. .NET only receives the HTML.
const instances = new WeakMap();

const ALLOWED_TAGS = new Set(['P', 'DIV', 'H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'UL', 'OL', 'LI', 'STRONG', 'B', 'EM', 'I', 'U', 'BR', 'SPAN']);

function sanitize(html) {
    const doc = new DOMParser().parseFromString(html ?? '', 'text/html');
    doc.body.querySelectorAll('script, style, iframe, object, embed, link, meta').forEach(el => el.remove());
    doc.body.querySelectorAll('*').forEach(el => {
        [...el.attributes].forEach(attr => {
            const name = attr.name.toLowerCase();
            if (name.startsWith('on') || name === 'srcdoc' || (name === 'href' && attr.value.trim().toLowerCase().startsWith('javascript:')))
                el.removeAttribute(attr.name);
        });
        if (!ALLOWED_TAGS.has(el.tagName)) {
            // Unwrap unknown elements but keep their content.
            el.replaceWith(...el.childNodes);
        }
    });
    return doc.body.innerHTML;
}

function updateActive(editor, toolbar) {
    const probes = {
        bold: () => document.queryCommandState('bold'),
        italic: () => document.queryCommandState('italic'),
        underline: () => document.queryCommandState('underline'),
        insertUnorderedList: () => document.queryCommandState('insertUnorderedList'),
        insertOrderedList: () => document.queryCommandState('insertOrderedList'),
        h3: () => (document.queryCommandValue('formatBlock') || '').toLowerCase() === 'h3',
    };
    toolbar.querySelectorAll('button[data-cmd]').forEach(button => {
        const probe = probes[button.dataset.cmd];
        let active = false;
        if (probe) {
            try { active = probe(); } catch { active = false; }
        }
        button.classList.toggle('rte-active', active);
    });
}

export function init(editor, toolbar, dotnetRef, html, readOnly) {
    editor.innerHTML = sanitize(html);
    editor.contentEditable = readOnly ? 'false' : 'true';

    const notify = () => dotnetRef.invokeMethodAsync('OnContentChanged', editor.innerHTML);

    const onInput = () => notify();

    const onToolbarMouseDown = (e) => {
        // Keep the selection in the editor while clicking toolbar buttons.
        if (e.target.closest('button[data-cmd]'))
            e.preventDefault();
    };

    const onToolbarClick = (e) => {
        const button = e.target.closest('button[data-cmd]');
        if (!button || editor.contentEditable !== 'true')
            return;

        const cmd = button.dataset.cmd;
        if (cmd === 'h3') {
            const isH3 = (document.queryCommandValue('formatBlock') || '').toLowerCase() === 'h3';
            document.execCommand('formatBlock', false, isH3 ? 'p' : 'h3');
        } else {
            document.execCommand(cmd, false, null);
        }
        editor.focus();
        updateActive(editor, toolbar);
        notify();
    };

    const onSelectionChange = () => {
        if (document.activeElement === editor)
            updateActive(editor, toolbar);
    };

    editor.addEventListener('input', onInput);
    toolbar.addEventListener('mousedown', onToolbarMouseDown);
    toolbar.addEventListener('click', onToolbarClick);
    document.addEventListener('selectionchange', onSelectionChange);

    instances.set(editor, { toolbar, onInput, onToolbarMouseDown, onToolbarClick, onSelectionChange });
}

export function setContent(editor, html) {
    editor.innerHTML = sanitize(html);
}

export function setReadOnly(editor, readOnly) {
    editor.contentEditable = readOnly ? 'false' : 'true';
    const instance = instances.get(editor);
    instance?.toolbar.classList.toggle('rte-hidden', readOnly);
}

export function destroy(editor) {
    const instance = instances.get(editor);
    if (!instance)
        return;

    editor.removeEventListener('input', instance.onInput);
    instance.toolbar.removeEventListener('mousedown', instance.onToolbarMouseDown);
    instance.toolbar.removeEventListener('click', instance.onToolbarClick);
    document.removeEventListener('selectionchange', instance.onSelectionChange);
    instances.delete(editor);
}

export function focusEditor(editor) {
    editor.focus();
}
