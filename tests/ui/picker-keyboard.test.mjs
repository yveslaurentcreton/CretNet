import { readFile } from 'node:fs/promises';
import assert from 'node:assert/strict';
import test from 'node:test';

const source = await readFile(new URL('../../src/CretNet.Platform.Blazor.Ui/Components/CnPicker.razor.js', import.meta.url), 'utf8');
const picker = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

class PickerInput extends EventTarget {
    open = true;
    disabled = false;
    readOnly = false;
    getAttribute(name) { return name === 'aria-expanded' ? String(this.open) : null; }
}

function key(input, name) {
    const event = new Event('keydown', { cancelable: true, bubbles: true });
    Object.defineProperty(event, 'key', { value: name });
    input.dispatchEvent(event);
    return event;
}

test('Enter cancels implicit form submission before selection closes the popup', () => {
    const input = new PickerInput();
    picker.connect?.(input);
    let selected = false;
    input.addEventListener('keydown', event => {
        if (event.key === 'Enter') {
            selected = true;
            input.open = false;
        }
    });
    const event = key(input, 'Enter');
    assert.equal(selected, true, 'the selection handler must still receive Enter');
    assert.equal(event.defaultPrevented, true, 'Enter must not implicitly submit the enclosing form');
});

test('arrows do not swallow the next character or Tab', () => {
    const input = new PickerInput();
    picker.connect?.(input);
    assert.equal(key(input, 'ArrowDown').defaultPrevented, true);
    for (const name of ['x', 'Backspace', 'Tab'])
        assert.equal(key(input, name).defaultPrevented, false, name);
    input.open = false;
    assert.equal(key(input, 'Enter').defaultPrevented, false, 'a closed picker does not own form submission');
});

test('read-only inputs and disconnected controls keep their native behavior', () => {
    const input = new PickerInput();
    picker.connect?.(input);
    picker.connect?.(input);
    input.readOnly = true;
    assert.equal(key(input, 'Enter').defaultPrevented, false);
    input.readOnly = false;
    picker.disconnect?.(input);
    assert.equal(key(input, 'Enter').defaultPrevented, false);
});
