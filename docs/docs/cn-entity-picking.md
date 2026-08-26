# Picking an entity

Two components answer "which one?", and they are meant to be used together.

`CnPicker` is the field: a combobox you type into, backed by a server-paged
provider. `CnPickerGridDialog` is its escape hatch: a real grid with search,
sorting and paging, for when typeahead is not enough because you are looking
for the one from last March rather than the one you can name.

```razor
<CnPicker Value="TaskId" ValueChanged="TaskIdChanged"
          Label="Task"
          Provider="SearchAsync"
          ResolveLabel="ResolveLabelAsync"
          OnAdd="AddAsync"
          AdvancedSearch="AdvancedSearchAsync" />
```

## The dropdown footer

`CnPicker`'s dropdown grows a footer as soon as you hand it one of two
callbacks. Both are optional and independent:

| Parameter | What appears | What it does |
|---|---|---|
| `AdvancedSearch` | A magnifier next to the field, and **Advanced search…** in the footer | Awaits your callback; a non-null result becomes the selection |
| `OnAdd` | **New…** in the footer | Awaits your callback; a non-null result becomes the selection |

Neither callback knows anything about how you find or create the thing. The
picker closes, hands control to you, and takes back whatever
`CnPickerItem` you return. That is deliberate: quick-create in one app opens
a dialog, in another it posts straight to an endpoint, and the picker should
not care.

Pass neither and there is no footer at all.

## The advanced-search dialog

`CnPickerGridDialog<TRow>` is generic over the row type. You give it a
provider and a set of column declarations; a row click closes the dialog with
that row.

```csharp
private async Task<CnPickerItem?> AdvancedSearchAsync()
{
    var row = await DialogService.ShowAsync<CnPickerGridDialog<TaskRow>, TaskRow>(
        Smart.Format(CnpLabels.SearchEntity, new { Entity = Labels.Task }),
        new()
        {
            [nameof(CnPickerGridDialog<TaskRow>.Provider)] =
                (Func<CnGridRequest, Task<CnGridPage<TaskRow>>>)LoadPageAsync,
            [nameof(CnPickerGridDialog<TaskRow>.Columns)] = TaskGrid.Columns,
        },
        width: "820px");

    return row is null ? null : new CnPickerItem(row.Id, row.Name);
}
```

Note what `Columns` is in that example: **the same fragment the entity's own
grid renders**. That is the whole point of the component being generic. Give
it a separate set of columns and the search dialog starts drifting from the
list people already know — different columns, different order, different
formatting, all describing the same thing. Give it the grid's own fragment
and it cannot.

## Where the pieces belong

`CnPicker` and `CnPickerGridDialog` know nothing about your entities. They
are not meant to be used raw from a screen either. The shape that works is
one component per entity in the app:

```
CretNet          CnPicker                CnPickerGridDialog        CnDataGrid
                     ▲                          ▲                      ▲
app (one per        TaskPicker  ───────────────►│                  TaskGrid
entity)          ShowAdd, ShowAdvanced          └── shares ────────► Columns
                     ▲                                                 ▲
screens          <TaskPicker ShowAdd />                        <TaskGrid ProjectId="…" />
```

The entity component is the only place that knows a task's provider, its
columns, its create flow and its labels. Screens pass parameters and nothing
else. Adding advanced search to every screen that picks a task is then one
line in one file.

## Localisation

Every string these components render is a parameter with an English default,
because the RCL carries no resource dependency. Hosts pass their own:

| Component | Parameters |
|---|---|
| `CnPicker` | `Placeholder`, `NothingFoundLabel`, `AddLabel`, `AdvancedSearchLabel`, `RecentGroupLabel`, `AllGroupLabel` |
| `CnPickerGridDialog` | `SearchPlaceholder`, `EmptyText`, `PreviousPageTitle`, `NextPageTitle` |
| `CnDataGrid` | the same four |

`CnpLabels` carries SmartFormat templates for the entity-shaped ones, so a
host writes the entity name once:

```csharp
Smart.Format(CnpLabels.SelectEntity, new { Entity = Labels.Task })  // "Select task…"
Smart.Format(CnpLabels.SearchEntity, new { Entity = Labels.Task })  // "Search task…"
Smart.Format(CnpLabels.NewEntity,    new { Entity = Labels.Task })  // "New task…"
```

Leaving the defaults in place is not a neutral choice — it means an English
"Select…" in a Dutch screen. The entity component is where you pass them, once.

## Rows

`CnPickerItem` is what a picker row is made of:

| Field | Renders as |
|---|---|
| `Label` | The title line, and the field's committed text |
| `Context` | A muted second line — "customer · project" and the like |
| `Meta` / `MetaAccent` | A badge on the right; accent makes it green |
| `Recent` | Groups the row under "Recent" when the query is empty |

`ItemTemplate` replaces the row entirely when that is not enough — an avatar,
a colour swatch, two badges. The default row covers most cases; reach for the
template when the entity genuinely looks different, not to reorder the same
three fields.
