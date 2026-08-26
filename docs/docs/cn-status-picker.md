# Changing a status

`CnStatusPicker` shows what state something is in, and — when the caller
allows it — changes it right where it is read.

```razor
<CnStatusPicker TStatus="ProjectStatus"
                Value="_project.Status"
                ValueChanged="ChangeStatusAsync"
                Options="_statusOptions"
                CanChange
                Loading="_saving"
                Prefix="Status" />
```

## Why not a dialog

A dialog for a single enum is a detour. It hides the value behind a click,
asks you to look at a dropdown that repeats what the row already said, and
then makes you confirm. The pill is the badge *and* the control: you read
the status where it belongs, and the same element opens the destinations.

When `CanChange` is false the component renders the identical pill as a
plain span. Nothing shifts, nothing resizes — a reader and an editor see the
same object.

## The caller owns the transitions

`Options` is a list the caller has already filtered. The component never
decides that "Cancelled" cannot become "Active"; that is a domain rule and
it lives with the domain.

```csharp
private static readonly IReadOnlyList<CnStatusOption<ProjectStatus>> _statusOptions =
[
    new(ProjectStatus.Draft,     "Draft"),
    new(ProjectStatus.Active,    "Active",    CnStatusTone.Accent),
    new(ProjectStatus.OnHold,    "On hold",   CnStatusTone.Warning),
    new(ProjectStatus.Completed, "Completed"),
    new(ProjectStatus.Cancelled, "Cancelled", CnStatusTone.Danger),
];
```

| Field | Does |
|---|---|
| `Value` | The status |
| `Label` | What to call it — localise here, the component never does |
| `Tone` | `Neutral`, `Accent`, `Warning` or `Danger` |
| `Disabled` | Visible but unreachable, for a transition a rule forbids |
| `Description` | One line under the label in the menu |

`Disabled` exists so a forbidden destination can still be *seen*. Silently
dropping an option leaves the reader wondering whether they misremembered
the workflow; showing it greyed answers the question.

## Tone is a property of the status, not of the theme

Each tone sets one custom property, `--status-color`, and the dot, the
frame, the tint and the value all derive from it. That is why a tone stays
coherent in both themes and why adding one is a single line of CSS rather
than four.

Reserve `Accent` for the state that means "this is live". If everything is
accented, nothing is.

## Parameters

| Parameter | Default | Does |
|---|---|---|
| `Value`, `ValueChanged` | — | The bound status |
| `Options` | — | The destinations, in reading order |
| `CanChange` | `false` | False renders a read-only badge |
| `Loading` | `false` | Blocks input while a change is in flight |
| `Prefix` | — | A word before the value, e.g. "Status" |
| `AriaLabel`, `MenuLabel`, `CloseLabel` | English | Chrome strings — pass localised text |
| `Class` | — | Extra class on the wrapper |

`Loading` matters more than it looks: without it a slow save leaves the pill
showing the old value while the user clicks a second destination.

## Keyboard

Arrows move between destinations and skip disabled ones, `Home` and `End`
jump to the ends, `Escape` closes and returns focus to the trigger. Opening
the menu focuses the current value, so the starting point is always where
you are, not the top of the list.
