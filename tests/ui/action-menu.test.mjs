import { readFile } from 'node:fs/promises';
import assert from 'node:assert/strict';
import test from 'node:test';

const source = await readFile(new URL('../../src/CretNet.Platform.Blazor.Ui/Components/CnActionMenu.razor.js', import.meta.url), 'utf8');
const { position, show, hide } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

const listeners = [];
function browser(width, height) {
    listeners.length = 0;
    globalThis.window = {
        innerWidth: width,
        innerHeight: height,
        addEventListener: (type, handler) => listeners.push({ type, handler }),
        removeEventListener: (type, handler) => {
            const index = listeners.findIndex(entry => entry.type === type && entry.handler === handler);
            if (index >= 0) listeners.splice(index, 1);
        },
    };
    globalThis.requestAnimationFrame = callback => { callback(); return 1; };
    globalThis.cancelAnimationFrame = () => { };
}

function list(width = 220, height = 160) {
    return {
        style: {},
        dataset: {},
        isConnected: true,
        get offsetWidth() { return width; },
        get offsetHeight() {
            const cap = Number.parseFloat(this.style.maxHeight);
            return Number.isNaN(cap) ? height : Math.min(height, cap);
        },
    };
}

const trigger = rect => ({ isConnected: true, getBoundingClientRect: () => rect });

test('opens under the trigger, right-aligned to it, when there is room', () => {
    browser(1280, 800);
    const menu = list();
    position(menu, trigger({ top: 100, bottom: 132, left: 900, right: 932 }), false);
    assert.equal(menu.style.position, 'fixed');
    assert.equal(menu.style.top, '136px');
    assert.equal(menu.style.left, `${932 - 220}px`);
    assert.equal(menu.dataset.placement, 'below');
});

test('flips above a trigger near the bottom of the viewport', () => {
    browser(1280, 800);
    const menu = list();
    position(menu, trigger({ top: 740, bottom: 772, left: 900, right: 932 }), false);
    assert.equal(menu.dataset.placement, 'above');
    assert.equal(menu.style.top, `${740 - 4 - 160}px`);
    assert.equal(menu.style.bottom, 'auto', 'top and bottom must not stretch the list');
});

test('a right-aligned list near the left edge shifts inward', () => {
    browser(1280, 800);
    const menu = list();
    position(menu, trigger({ top: 100, bottom: 132, left: 10, right: 42 }), false);
    assert.equal(menu.style.left, '8px');
});

test('a start-aligned list near the right edge flips left inside the viewport', () => {
    browser(400, 800);
    const menu = list();
    position(menu, trigger({ top: 100, bottom: 132, left: 360, right: 392 }), true);
    assert.ok(Number.parseFloat(menu.style.left) + 220 <= 392);
});

test('a long list in a short viewport is capped to the room it has', () => {
    browser(1280, 400);
    const menu = list(220, 600);
    position(menu, trigger({ top: 40, bottom: 72, left: 900, right: 932 }), false);
    assert.equal(menu.dataset.placement, 'below');
    assert.ok(Number.parseFloat(menu.style.top) + menu.offsetHeight <= 400 - 8);
});

function liftable(order) {
    const attributes = new Map();
    return {
        ...list(),
        open: false,
        hasAttribute: name => attributes.has(name),
        setAttribute: (name, value) => attributes.set(name, value),
        getAttribute: name => attributes.get(name),
        matches(selector) { return selector === ':popover-open' && this.open; },
        showPopover() { this.open = true; order.push(this); },
    };
}

test('show lifts backdrop then list into the top layer once, and hide stops following', () => {
    browser(1280, 800);
    const order = [];
    const backdrop = liftable(order);
    const menu = liftable(order);
    const anchor = trigger({ top: 100, bottom: 132, left: 900, right: 932 });

    show('m1', menu, backdrop, anchor, false);
    show('m1', menu, backdrop, anchor, false); // a re-render while open

    assert.deepEqual(order, [backdrop, menu]);
    assert.equal(menu.getAttribute('popover'), 'manual');
    assert.equal(backdrop.getAttribute('popover'), 'manual');
    assert.equal(listeners.length, 2, 'one scroll and one resize listener');

    // The trigger moves (its container scrolled): the list follows it.
    anchor.getBoundingClientRect = () => ({ top: 300, bottom: 332, left: 900, right: 932 });
    listeners.find(entry => entry.type === 'scroll').handler();
    assert.equal(menu.style.top, '336px');

    hide('m1');
    assert.equal(listeners.length, 0);
});

test('without the Popover API the list is still pinned with fixed coordinates', () => {
    browser(1280, 800);
    const menu = list();
    show('m2', menu, null, trigger({ top: 100, bottom: 132, left: 900, right: 932 }), false);
    assert.equal(menu.style.position, 'fixed');
    assert.equal(menu.style.top, '136px');
    hide('m2');
});
