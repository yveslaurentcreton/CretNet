# Toasts and notifications

Two ways of telling someone something, and they are not interchangeable.

A **toast** is the receipt for something the person just did. It floats, it
fades, and losing it costs nothing — the result is already on their screen.

A **notification** is the record of something that happened without them. It
waits in the bell until they deal with it, because if they miss it, it is
gone.

The line between them is the only design rule here that matters. Get it
wrong and the bell fills with "Saved" while a failed import scrolls past in
two seconds.

|  | Toast | Notification |
|---|---|---|
| About | Something *you* did | Something that happened *without you* |
| Lifetime | Seconds | Until you deal with it |
| Missing it | Fine — the result is on screen | Not fine — that is why it is durable |
| Where | Floating, out of the way | In the bell, with a count |

One crossing is allowed: a notification arriving while the person is looking
at the app may also pass by as a toast. Never the reverse — a confirmation of
your own click does not belong in an inbox.

## Toasts

```razor
@* once, in the layout *@
<CnToastHost Position="CnToastPosition.BottomRight" />
```

```csharp
Toasts.Error("Task not saved", "The deadline is before the start date.");
Toasts.Success("Time entry saved", "1 h 24 m on 26.002.");
Toasts.Show(CnToastSeverity.Information, "Task deleted", null,
    actionLabel: "Undo", action: RestoreAsync);
```

**Position is a parameter for a reason.** A fixed corner will eventually sit
on top of something that matters — in WAM the top-right default landed
squarely on the running timer, the bell and the avatar. Six anchors are
available; bottom-right is the default because it stays clear of a topbar and
sits near where the pointer already is after a save.

### What the host does for you

- **The newest toast is nearest its corner.** Top-anchored stacks reverse to
  achieve that; bottom-anchored ones already grow the right way. A burst
  never asks you to start reading at the far end.
- **Beyond `MaxVisible`, toasts queue rather than drop** — and a queued toast
  does not start its countdown until it is actually on screen. Otherwise a
  burst quietly expires things nobody ever saw.
- **Hovering pauses the countdown.** Reading a toast should not cost you the
  toast.
- **Errors last longer** than the rest (`ErrorDuration`), because they are the
  ones worth reading twice.

### Severity

The colour lives in a 3px rail and the icon, never the whole card. A saved
record and a failed import must not shout equally loudly.

| Severity | Token |
|---|---|
| Success | `--cn-accent` |
| Error | `--cn-danger` |
| Warning | `--cn-warn` |
| Information | `--cn-info` |

### One action, at most

`ActionLabel` + `Action` add a single link — "Undo", "Open". One,
deliberately: a toast that needs a choice is a dialog wearing the wrong
clothes, and it will disappear halfway through the decision.

## Notifications

```razor
@* in the topbar — the bell owns its panel *@
<CnNotificationBell OnOpen="GoTo" Title="@Labels.Notifications" />
```

```csharp
// the host supplies persistence and authorization
services.AddScoped<ICnNotificationClient, MyNotificationClient>();
```

`CnNotificationState` is the engine: one filter, one page, one summary. Read
and archive apply locally first and confirm against the client afterwards — a
row that only reacts after a round trip feels broken.

`CnNotificationBell` polls the *summary* every 30 seconds by default, not the
page: counting is cheap, listing is not. Set `PollInterval` to zero for a host
that pushes instead.

### The row

Unread sits above read, under **NEW** and **EARLIER**. Opening a row marks it
read and raises `OnOpen` — the component does not know what a route is, so the
host navigates.

The round avatar carries the **severity**, not a decorative glyph: it is the
one thing read before the words, and that slot is too valuable to waste. The
category is a chip, and time is relative ("7 h ago"), because an inbox answers
"how long has this been sitting here", not "what time was it".

`RequiresAction` is not the same as unread. You can have read a failure and
still not have fixed it — which is why the Action filter counts separately.

### What the host must persist

The field that cannot be added later without rewriting everything is a
**stable correlation key**. One event that reports itself five times is one
row that updates five times, not five rows. Without it, a backup that retried
four times fills the bell four times.

Beyond that: category, severity, type, title, message, action path, subject,
`RequiresAction`, and the occurred/read/archived moments.

## Localisation

Every string both components render is a parameter with an English default —
the RCL carries no resource dependency. `CnNotificationBell` passes its labels
straight through to the panel, so a host localises in one place.

Two of them are functions rather than strings, because they are decisions and
not words: `EmptyLabel` (nothing archived and all-caught-up are different
kinds of empty) and `FormatWhen`.

## Panels

`CnPanel` is the drawer both of these needed and the dialog host does not
provide — `CnDialogService` centres its dialogs. Anything that slides in from
an edge uses `.cn-panel` with `.cn-panel--right` or `--left`, over
`.cn-panel-scrim`.
