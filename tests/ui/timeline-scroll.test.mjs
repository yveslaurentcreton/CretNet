import { readFile } from 'node:fs/promises';
import assert from 'node:assert/strict';
import test from 'node:test';

const source = await readFile(new URL('../../src/CretNet.Platform.Blazor.Ui/Components/CnTimeline.razor.js', import.meta.url), 'utf8');
const { viewOf, todayLeft, centredLeft, readZoom, writeZoom } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

// 56 months of 60px in a 720px view (a year): the track is 3360px wide.
const metrics = (left, extra = {}) => ({ trackWidth: 3360, viewWidth: 720, months: 56, today: 0.558756, left, max: 3360 - 720, ...extra });

test('today starts a third from the left of the view', () => {
    const left = todayLeft(metrics(0));
    assert.equal(Math.round(left), Math.round(0.558756 * 3360 - 240));
    const view = viewOf(metrics(left));
    assert.deepEqual(view, { first: 27, last: 38, atStart: false, atEnd: false });
});

test('the start position is clamped to the track', () => {
    assert.equal(todayLeft(metrics(0, { today: 0.01 })), 0);
    assert.equal(todayLeft(metrics(0, { today: 1 })), 3360 - 720);
    assert.equal(todayLeft(metrics(0, { today: Number.NaN })), 0, 'no today: start at the beginning');
});

test('the view reports its months and whether an end is reached', () => {
    assert.deepEqual(viewOf(metrics(0)), { first: 0, last: 11, atStart: true, atEnd: false });
    assert.deepEqual(viewOf(metrics(2640)), { first: 44, last: 55, atStart: false, atEnd: true });
    // Everything in one view: both ends at once.
    assert.deepEqual(viewOf(metrics(0, { trackWidth: 720, max: 0 })), { first: 0, last: 55, atStart: true, atEnd: true });
});

test('a zoom keeps the day in the middle of the view in place', () => {
    // Middle at 40 % of the track; zoomed to three years the track is 1120px.
    const left = centredLeft(metrics(0, { trackWidth: 1120, max: 400 }), 0.4);
    assert.equal(left, 0.4 * 1120 - 360);
    assert.equal(centredLeft(metrics(0), 0.01), 0, 'clamped at the start');
});

test('the zoom is remembered per key, and storage failures are harmless', () => {
    const store = new Map();
    globalThis.localStorage = { getItem: key => store.get(key) ?? null, setItem: (key, value) => store.set(key, value) };
    assert.equal(readZoom('item-price'), null);
    writeZoom('item-price', 36);
    assert.equal(store.get('cn-timeline-zoom:item-price'), '36');
    assert.equal(readZoom('item-price'), 36);
    assert.equal(readZoom('subscription'), null);

    globalThis.localStorage = { getItem: () => { throw new Error('blocked'); }, setItem: () => { throw new Error('blocked'); } };
    assert.equal(readZoom('item-price'), null);
    assert.doesNotThrow(() => writeZoom('item-price', 6));
    delete globalThis.localStorage;
});
