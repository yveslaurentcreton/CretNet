# Cn date fields

Four ways a Cn screen asks about time, all built from the same parts: one
masked input, one calendar, one clock. A range is the date field twice in one
pill; a date-and-time is the date field beside the time field. One
implementation, four shapes — so the typing rules, the keyboard behaviour and
the visual language cannot drift apart.

```razor
@* one date *@
<CnDateField Label="Deadline" @bind-Value="Deadline" />

@* a period *@
<CnDateRangeField @bind-From="From" @bind-To="To" Presets="Presets" />

@* a time of day, optionally to the second *@
<CnTimeField Label="Start" @bind-Value="Start" Seconds />

@* a moment: date and time in one pill *@
<CnDateTimeField Label="Start" @bind-Value="Start" Seconds />
```

## Why not the native date input

The browser's `input type="date"` looks close enough until you use it: it
stretches its text across the field and pins its calendar button to the far
right, so a pair of them reads as four loose pieces; its picker cannot be
styled, cannot show a range, and cannot be told what "this month" means. The
Cn fields keep the one thing the native input got right — you can type into it
— and take back the rest.

## Typing

Digits are the only truth. Everything else you type is noise: separators,
pasted text and stray characters all reduce to the digits they carry, so
pasting `01/09/2026 - 30/09/2026` works as well as typing it.

Separators are drawn **between** digits and only once the digit behind them
exists. That is what keeps backspace honest: there is never a lone `/` at the
end that has to be deleted twice.

| You type | The field shows |
| :--- | :--- |
| `1` | `1` |
| `10` | `10` |
| `100` | `10/0` |
| `1008` | `10/08` |
| `10082026` | `10/08/2026` |

Typing a separator yourself after a single digit means what you would expect:
`1/` becomes `01/`. Years are never padded that way — a half-typed year is not
a padding candidate.

### What Tab fills in

Leaving the field — Tab, Enter, or clicking elsewhere — completes what you
started. Missing parts come from today; values that cannot exist are pulled
back to the nearest day that can. This is what makes the control quick: most
dates are four keystrokes.

| You type | You get | Why |
| :--- | :--- | :--- |
| `0101` | 01/01/2026 | no year typed → this year |
| `3101` | 31/01/2026 | January really has 31 days |
| `3106` | 30/06/2026 | June has 30 → pulled back |
| `3102` | 28/02/2026 | February 2026 has 28 |
| `3113` | 31/12/2026 | there is no month 13 → December |
| `5` | 05/08/2026 | a day on its own → this month |
| `010227` | 01/02/2027 | a two-digit year → 20xx |

An impossible **complete** date behaves differently from an impossible partial
one: while you are still typing, `32/13/2026` marks the field invalid and
leaves your digits alone so you can fix the wrong one. Only leaving the field
applies the rules above.

The rules live in `CnDateMask` as pure functions and are covered by
`CnDateMaskTests`; the components never re-implement them.

### In a range

The two halves work exactly the same, with Tab stepping between them:

```
0101  Tab  3101  Tab   →   01/01/2026 – 31/01/2026
```

Typing straight through works too — a ninth digit flows into the second half
by itself, so `1008202618082026` is a complete range without touching Tab.
Backspace at the start of the second half hops back to the first. A pair typed
the wrong way round is still a period: the earliest day becomes the start.

## The calendar

Clicking the field opens it. A range is dragged out: the first click sets the
start, the days colour along under your cursor, the second click closes it.
Moving before the start flips the range rather than refusing it.

The band is painted per cell, so the tint runs on uninterrupted across a week
while every day stays a circle; it rounds off where the band actually stops —
at the ends of the range and at the edges of each week row.

### Zooming out

Click the month title and you get the twelve months of that year; click the
year and you get twelve years. Choosing a year takes you back to its months,
choosing a month back to its days. The arrows step by whatever the level
shows: a month, a year, or a block of twelve years.

The popover keeps exactly the same size at every level, so nothing jumps under
your cursor. Reopening always starts at the days again — a zoom level is a way
to navigate, not a state worth remembering.

## Times and moments

`CnTimeField` follows the same rules with two segments instead of three, plus
one that belongs to a clock: **a first digit above 2 cannot start a two-digit
hour**, so it is the whole hour and the next digit already belongs to the
minutes. That is what makes `930` read as half past nine.

| You type | You get | Why |
| :--- | :--- | :--- |
| `9` | 09:00 | an hour on its own → the top of it |
| `930` | 09:30 | 9 cannot start a two-digit hour |
| `1345` | 13:45 | plain hh:mm |
| `2570` | 23:59 | pulled back to the nearest time that exists |
| `045` | 04:05 | |

`Seconds` turns the field into hh:mm:ss — mask, completion and the dial all
follow the flag. The rules live in `CnTimeMask`, covered by `CnTimeMaskTests`.

### The clock dial

The phone idiom: hours on a face with 12 at the top, the inner ring carrying
00 and 13–23 so a whole day fits without an am/pm switch, then minutes and —
when asked for — seconds with 00 at the top and 30 at the bottom. Tapping or
dragging anywhere on the face counts: the angle is the value, the numbers are
only there to read it by. Each part hands over to the next by itself.

The dial's centre is read once when the popover opens and the angles are
computed in C#; asking the browser for the rect on every pointer move would
mean an interop call per pixel of drag.

### CnDateTimeField

Date and time side by side in one pill. Typing runs straight through: a
finished date moves the caret to the time and turns the popover into the
clock, and backspacing out of an empty time carries on in the date. Picking a
day does the same — the time is the half still missing, so the calendar hands
over rather than closing on a half-finished value.

The value is a single `DateTime?`. A date without a time is held internally
but not published: half a moment is not a moment.

## Closing and clearing

The calendar closes when focus leaves the control — clicking elsewhere, or
tabbing past it. Moving between the two halves of a range does not count as
leaving, which is why the check waits a tick rather than reacting to the bare
blur (Blazor does not surface `relatedTarget`, so there is no way to ask where
focus went).

Emptying a range needs its own affordance: `Ctrl+A` only ever selects the half
the caret sits in, so a pill holding a value shows a small **×** that clears
both halves at once. The single field has the same button, plus *Clear* in the
calendar footer.

Inside a dialog the popover would be clipped by the scrolling body, so while it
is open it is promoted to fixed viewport coordinates and flips above the field
when there is more room up there.

## Keyboard

| Key | Does |
| :--- | :--- |
| Tab | completes the date and moves on (to the second half of a range) |
| Enter | completes and closes |
| Escape | puts back the value that was there when the popover opened |
| ↓ | moves from the field into the calendar |
| Backspace | steps back digit by digit, over the separators; at the start of a range's second half it hops to the first |

Tabbing into a field selects its whole value, so typing replaces it. Clicking
puts the caret where you clicked, for fixing one digit.

## Parameters

### CnDateField

Inherits `CnInputBase<DateTime?>`, so `Value`, `ValueChanged`, `Label`, `For`,
`ReadOnly`, `Disabled`, `Subtle`, `Class` and `Style` behave as on every other
Cn input, including inline validation through `For`.

| Parameter | Default | Meaning |
| :--- | :--- | :--- |
| `MinDate` / `MaxDate` | `null` | days outside the window are shown but not selectable |
| `Placeholder` | the format hint | text while the field is empty |
| `DateFormatHint` | `dd/mm/yyyy` | the shape shown as placeholder |
| `TodayLabel`, `ClearLabel`, `PickHint`, `PreviousMonthLabel`, `NextMonthLabel` | English | chrome strings; hosts pass their own words |

### CnDateRangeField

| Parameter | Default | Meaning |
| :--- | :--- | :--- |
| `From` / `FromChanged`, `To` / `ToChanged` | `null` | the two ends, bindable |
| `MonthCount` | `2` | months side by side in the popover |
| `Presets` | empty | quick picks down the left; empty hides the column |
| `MinDate` / `MaxDate` | `null` | selectable window |
| `Label`, `ReadOnly`, `Disabled`, `Class`, `Style` | — | as elsewhere |
| `FromLabel`, `ToLabel`, `ClearLabel`, `PickStartHint`, `PickEndHint`, `DayLabel`, `DaysLabel`, `PreviousMonthLabel`, `NextMonthLabel`, `DateFormatHint` | English | chrome strings |

Presets are plain data, so a host decides what "this month" means:

```csharp
private IReadOnlyList<CnDateRangePreset> Presets =>
[
    new("This month", firstOfMonth, lastOfMonth),
    new("Last 7 days", today.AddDays(-6), today),
];
```

## Culture

Day names, month names and the first day of the week come from
`CultureInfo.CurrentCulture`, so a Belgian user gets a Monday-first calendar
with Dutch month names. The typing mask is day-month-year; a culture that
writes month-day-year needs the mask segments reordered — that is the one
place where the current implementation is Europe-shaped, and it is contained
in `CnDateMask`.

## Anatomy

| Piece | Lives in | Role |
| :--- | :--- | :--- |
| `CnDateMask` | `Components/CnDateMask.cs` | the typing rules, as pure functions |
| `CnDateInput` | `Components/CnDateInput.razor` | one masked input, no chrome |
| `CnCalendarPanel` | `Components/CnCalendarPanel.razor` | the three zoom levels and the band |
| `CnDateField` | `Components/CnDateField.razor` | one date: input + calendar |
| `CnDateRangeField` | `Components/CnDateRangeField.razor` | two inputs in one pill + calendar + presets |
| `CnTimeMask` | `Components/CnTimeMask.cs` | the time rules, as pure functions |
| `CnTimeInput` | `Components/CnTimeInput.razor` | one masked time input, no chrome |
| `CnClockPanel` | `Components/CnClockPanel.razor` | the dial: hours, minutes, seconds |
| `CnTimeField` | `Components/CnTimeField.razor` | one time: input + dial |
| `CnDateTimeField` | `Components/CnDateTimeField.razor` | a moment: date + time, calendar handing over to the dial |
| `CnDateInput.razor.js` | same folder | writes the masked text and puts the caret back; re-exports the popover placement from `CnPicker.razor.js` |

The JavaScript does three things Blazor cannot: set a caret position, select a
field's text, and read an element's position on screen. Every rule about what
the text should *be* stays in C#.

One trap worth knowing: **never wrap these fields in a `<label>` element**.
A click anywhere inside a label focuses its form control, so clicking the
calendar or the dial would look to the field like the user clicking into it.
Use a `<div>` with a `<span class="cn-label">`, which is what the components
render themselves.

## Styling

All of it runs on the Cn tokens (`--cn-accent`, `--cn-card`, `--cn-stroke`,
`--cn-text-2`), so both themes follow automatically. The range band is derived
from the accent with `color-mix`, which means changing the accent changes the
band with it.

The inputs are sized to exactly what a full date renders. That is deliberate:
any slack piles up on one side of the dash and makes the pair look lopsided.
If you change the field's font size, revisit that width.
