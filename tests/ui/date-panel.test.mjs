import { readFile } from 'node:fs/promises';
import assert from 'node:assert/strict';
import test from 'node:test';

const source = await readFile(new URL('../../src/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js', import.meta.url), 'utf8');
const { placePanel, focus } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

function panel() {
    return {
        style: {},
        // Layout changes when the calendar leaves its absolute-positioned parent.
        get offsetHeight() { return this.style.position === 'fixed' ? 284 : 240; },
        get offsetWidth() { return 250; },
    };
}

test('calendar near the bottom fits after promotion changes its height', () => {
    globalThis.window = { innerHeight: 720, innerWidth: 1280 };
    const calendar = panel();
    placePanel(calendar, { getBoundingClientRect: () => ({ top: 406, bottom: 438, left: 881 }) });
    const top = Number.parseFloat(calendar.style.top);
    assert.ok(top >= 8);
    assert.ok(top + calendar.offsetHeight < 406, 'calendar should fit above the field');
    assert.equal(calendar.style.bottom, 'auto', 'CSS top and bottom must not stretch the calendar');
});

test('calendar keeps the usual position below a field with room', () => {
    globalThis.window = { innerHeight: 720, innerWidth: 1280 };
    const calendar = panel();
    placePanel(calendar, { getBoundingClientRect: () => ({ top: 50, bottom: 82, left: 100 }) });
    assert.equal(calendar.style.top, '88px');
    assert.equal(calendar.style.left, '100px');
});

test('calendar near the right edge stays inside a narrow viewport', () => {
    globalThis.window = { innerHeight: 720, innerWidth: 360 };
    const calendar = panel();
    placePanel(calendar, { getBoundingClientRect: () => ({ top: 50, bottom: 82, left: 300 }) });
    assert.ok(Number.parseFloat(calendar.style.left) + calendar.offsetWidth <= 352);
});

test('returning from an async date change tolerates a closed host', () => {
    focus(null, true);
    focus({ isConnected: false, focus() { assert.fail('detached input received focus'); } }, true);
    const calls = [];
    focus({ isConnected: true, focus() { calls.push('focus'); }, select() { calls.push('select'); } }, true);
    assert.deepEqual(calls, ['focus', 'select']);
});
