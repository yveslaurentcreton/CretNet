// Collocated JS module for CnRichTextEditor: owns the contenteditable behavior,
// toolbar commands and active-state highlighting. .NET only receives the HTML.
//
// S-348: the DOM is held to the allow-list of CnRichTextSanitizer (.NET) on
// every input, not only when content is set: p, br, h2, h3, strong/b, em/i,
// u, nested ul/ol/li, a[href] (http, https, mailto) and span[data-color] from
// the palette. Pasted HTML goes to .NET first, which also turns styles into
// tags and colours into the nearest palette colour before stripping them.
const instances = new WeakMap();

// Print colours of CnMarkdownPalette; the colour command round-trips through
// font[color] so the browser splits and overrides existing colours for us.
const PALETTE = { accent: '#128a30', blauw: '#3159a7', oranje: '#b35f00', rood: '#c42b2f' };
const PALETTE_BY_HEX = Object.fromEntries(Object.entries(PALETTE).map(([name, hex]) => [hex, name]));
const NO_COLOR = '#010101';

const ALLOWED = new Set(['P', 'BR', 'H2', 'H3', 'STRONG', 'B', 'EM', 'I', 'U', 'UL', 'OL', 'LI', 'A', 'SPAN']);
const TOP_LEVEL = new Set(['P', 'H2', 'H3', 'UL', 'OL']);
const DROPPED = new Set(['SCRIPT', 'STYLE', 'META', 'LINK', 'TITLE', 'HEAD', 'NOSCRIPT', 'TEMPLATE', 'IFRAME', 'OBJECT',
    'EMBED', 'SVG', 'MATH', 'CANVAS', 'VIDEO', 'AUDIO', 'INPUT', 'BUTTON', 'SELECT', 'TEXTAREA', 'IMG', 'XML', 'O:P']);
const BLOCKS = new Set(['P', 'DIV', 'SECTION', 'ARTICLE', 'BLOCKQUOTE', 'HEADER', 'FOOTER', 'MAIN', 'ASIDE', 'NAV', 'FIGURE',
    'FIGCAPTION', 'ADDRESS', 'CENTER', 'PRE', 'H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'UL', 'OL', 'LI', 'TABLE', 'THEAD',
    'TBODY', 'TFOOT', 'TR', 'TD', 'TH', 'HR', 'FORM', 'FIELDSET', 'DL', 'DT', 'DD']);
const SAFE_URL = /^\s*(https?:|mailto:)/i;

function rename(element, tag) {
    const replacement = element.ownerDocument.createElement(tag);
    replacement.append(...element.childNodes);
    element.replaceWith(replacement);
    return replacement;
}

function unwrap(element) {
    element.replaceWith(...element.childNodes);
}

function hasBlockChild(element) {
    return [...element.children].some(child => BLOCKS.has(child.tagName.toUpperCase()));
}

function paletteName(element) {
    const tag = element.tagName.toUpperCase();
    if (tag === 'SPAN')
        return Object.hasOwn(PALETTE, element.dataset.color ?? '') ? element.dataset.color : null;
    if (tag === 'FONT')
        return PALETTE_BY_HEX[(element.getAttribute('color') ?? '').toLowerCase()] ?? null;
    return null;
}

// Structural clean-up of one subtree (children first).
function clean(node) {
    for (const child of [...node.childNodes]) {
        if (child.nodeType === Node.COMMENT_NODE) {
            child.remove();
            continue;
        }
        if (child.nodeType !== Node.ELEMENT_NODE)
            continue;

        const tag = child.tagName.toUpperCase();
        if (DROPPED.has(tag)) {
            child.remove();
            continue;
        }

        clean(child);
        let element = child;
        if (tag === 'H1') {
            element = rename(element, 'h2');
        } else if (/^H[4-6]$/.test(tag)) {
            element = rename(element, 'h3');
        } else if (tag === 'SPAN' || tag === 'FONT') {
            const name = paletteName(element);
            if (!name) {
                unwrap(element);
                continue;
            }
            element = tag === 'FONT' ? rename(element, 'span') : element;
            [...element.attributes].forEach(attribute => element.removeAttribute(attribute.name));
            element.setAttribute('data-color', name);
            continue;
        } else if (tag === 'A') {
            if (!SAFE_URL.test(element.getAttribute('href') ?? '')) {
                unwrap(element);
                continue;
            }
        } else if (BLOCKS.has(tag) && !ALLOWED.has(tag)) {
            // div, section, Word paragraphs, table cells: keep the line break.
            if (hasBlockChild(element)) {
                unwrap(element);
                continue;
            }
            element = rename(element, 'p');
        } else if (!ALLOWED.has(tag)) {
            unwrap(element);
            continue;
        }

        [...element.attributes]
            .filter(attribute => !(element.tagName === 'A' && attribute.name === 'href'))
            .forEach(attribute => element.removeAttribute(attribute.name));
    }
}

// Loose text or inline elements between blocks go into their own paragraph.
function wrapLoose(root) {
    let paragraph = null;
    for (const child of [...root.childNodes]) {
        const isBlock = child.nodeType === Node.ELEMENT_NODE && TOP_LEVEL.has(child.tagName.toUpperCase());
        if (isBlock) {
            paragraph = null;
            continue;
        }
        if (child.nodeType === Node.TEXT_NODE && !child.nodeValue.trim() && !paragraph) {
            child.remove();
            continue;
        }
        if (!paragraph) {
            paragraph = root.ownerDocument.createElement('p');
            child.before(paragraph);
        }
        paragraph.append(child);
    }
}

export function sanitize(html) {
    const doc = new DOMParser().parseFromString(html ?? '', 'text/html');
    clean(doc.body);
    wrapLoose(doc.body);
    return doc.body.innerHTML;
}

function isClean(root) {
    for (const child of root.childNodes) {
        if (child.nodeType === Node.TEXT_NODE && child.nodeValue.trim())
            return false;
        if (child.nodeType === Node.COMMENT_NODE)
            return false;
        if (child.nodeType === Node.ELEMENT_NODE && !TOP_LEVEL.has(child.tagName))
            return false;
    }
    for (const element of root.querySelectorAll('*')) {
        if (!ALLOWED.has(element.tagName))
            return false;
        for (const attribute of element.attributes) {
            const allowed = (element.tagName === 'A' && attribute.name === 'href')
                || (element.tagName === 'SPAN' && attribute.name === 'data-color');
            if (!allowed)
                return false;
        }
        if (element.tagName === 'A' && !SAFE_URL.test(element.getAttribute('href') ?? ''))
            return false;
        if (element.tagName === 'SPAN' && !paletteName(element))
            return false;
    }
    return true;
}

// The caret as character offsets survives rebuilding the DOM.
function saveSelection(root) {
    const selection = document.getSelection();
    if (!selection?.rangeCount)
        return null;
    const range = selection.getRangeAt(0);
    if (!root.contains(range.startContainer) || !root.contains(range.endContainer))
        return null;
    const offset = (node, index) => {
        const probe = document.createRange();
        probe.selectNodeContents(root);
        probe.setEnd(node, index);
        return probe.toString().length;
    };
    return { start: offset(range.startContainer, range.startOffset), end: offset(range.endContainer, range.endOffset) };
}

function positionAt(root, offset) {
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
    let remaining = offset;
    let last = null;
    for (let node = walker.nextNode(); node; node = walker.nextNode()) {
        if (remaining <= node.length)
            return [node, remaining];
        remaining -= node.length;
        last = node;
    }
    return last ? [last, last.length] : [root, root.childNodes.length];
}

function restoreSelection(root, saved) {
    if (!saved)
        return;
    const [startNode, startOffset] = positionAt(root, saved.start);
    const [endNode, endOffset] = positionAt(root, saved.end);
    const range = document.createRange();
    range.setStart(startNode, startOffset);
    range.setEnd(endNode, endOffset);
    const selection = document.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
}

function normalize(editor) {
    if (isClean(editor))
        return;
    const saved = document.activeElement === editor ? saveSelection(editor) : null;
    editor.innerHTML = sanitize(editor.innerHTML);
    restoreSelection(editor, saved);
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

    const onInput = () => {
        normalize(editor);
        notify();
    };

    const onFocus = () => {
        // Enter makes paragraphs, never bare divs.
        try { document.execCommand('defaultParagraphSeparator', false, 'p'); } catch { /* unsupported */ }
    };

    const onPaste = async (e) => {
        const pasted = e.clipboardData?.getData('text/html');
        if (!pasted || editor.contentEditable !== 'true')
            return; // Plain text pastes natively and is normalised on input.

        e.preventDefault();
        const saved = saveSelection(editor);
        const clean = await dotnetRef.invokeMethodAsync('SanitizePaste', pasted);
        editor.focus();
        restoreSelection(editor, saved);
        if (clean)
            document.execCommand('insertHTML', false, clean);
        normalize(editor);
        notify();
    };

    const onToolbarMouseDown = (e) => {
        // Keep the selection in the editor while clicking toolbar buttons,
        // including the colour menu.
        if (e.target.closest('button'))
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
        normalize(editor);
        updateActive(editor, toolbar);
        notify();
    };

    const onSelectionChange = () => {
        if (document.activeElement === editor)
            updateActive(editor, toolbar);
    };

    editor.addEventListener('input', onInput);
    editor.addEventListener('focus', onFocus);
    editor.addEventListener('paste', onPaste);
    toolbar.addEventListener('mousedown', onToolbarMouseDown);
    toolbar.addEventListener('click', onToolbarClick);
    document.addEventListener('selectionchange', onSelectionChange);

    instances.set(editor, { toolbar, notify, onInput, onFocus, onPaste, onToolbarMouseDown, onToolbarClick, onSelectionChange });
}

// Colours the selection with a palette colour, or removes colour (null).
export function applyColor(editor, name) {
    const instance = instances.get(editor);
    const selection = document.getSelection();
    if (!instance || editor.contentEditable !== 'true' || !selection?.rangeCount || selection.isCollapsed)
        return;
    if (!editor.contains(selection.getRangeAt(0).commonAncestorContainer))
        return;

    let saved = saveSelection(editor);
    editor.querySelectorAll('span[data-color]').forEach(span => {
        const font = document.createElement('font');
        font.setAttribute('color', PALETTE[span.dataset.color] ?? NO_COLOR);
        font.append(...span.childNodes);
        span.replaceWith(font);
    });
    restoreSelection(editor, saved);

    try { document.execCommand('styleWithCSS', false, false); } catch { /* unsupported */ }
    document.execCommand('foreColor', false, PALETTE[name] ?? NO_COLOR);

    saved = saveSelection(editor);
    editor.innerHTML = sanitize(editor.innerHTML);
    restoreSelection(editor, saved);
    instance.notify();
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
    editor.removeEventListener('focus', instance.onFocus);
    editor.removeEventListener('paste', instance.onPaste);
    instance.toolbar.removeEventListener('mousedown', instance.onToolbarMouseDown);
    instance.toolbar.removeEventListener('click', instance.onToolbarClick);
    document.removeEventListener('selectionchange', instance.onSelectionChange);
    instances.delete(editor);
}

export function focusEditor(editor) {
    editor.focus();
}
