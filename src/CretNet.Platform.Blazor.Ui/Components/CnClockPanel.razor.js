// The clock's one piece of browser knowledge: where the dial sits. Pointer
// events carry viewport coordinates, so C# needs the centre to turn them into
// an angle; reading it once per open beats an interop call per pixel of drag.
export function centreOf(dial) {
    if (!dial) {
        return [0, 0];
    }
    const rect = dial.getBoundingClientRect();
    return [rect.left + rect.width / 2, rect.top + rect.height / 2];
}
