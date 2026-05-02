# Query-first bindings reference

Components that bind a UI control to a field on a `QueryState<TQuery>`.
Each one mutates the query via `RecordCloner.With(...)` (a runtime
`with`-equivalent), so you only spell out the getter and field-name
safety is compile-time (rename the property, the page fails to build).

## `<CnpBindCheckboxGroup TQuery TValue>`

Multi-value enum / lookup filter. Renders a stack of FluentCheckboxes
inside the `<FilterControls>` slot of `<CnpEntityDataGrid>`.

```razor
<CnpBindCheckboxGroup TQuery="MyTasksQuery" TValue="TaskStatus"
                      State="_queryState!"
                      Field="q => q.IncludeStatuses"
                      Category="Status"
                      Options="_statusOptions" />
```

Parameters:

| Name        | Type                                             | Required | Description                                                                |
| :---        | :---                                             | :---     | :---                                                                       |
| `State`     | `QueryState<TQuery>`                             | yes      | The per-screen state the page builds in `OnInitializedAsync`.              |
| `Field`     | `Expression<Func<TQuery, TValue[]?>>`            | yes      | Member-access expression for the array field — e.g. `q => q.IncludeStatuses`. |
| `Options`   | `IReadOnlyList<CheckboxOption<TValue>>`          | yes      | One option per checkbox (value + label).                                   |
| `Category`  | `string?`                                        | no       | Header label rendered above the group.                                     |

`CheckboxOption<TValue>(TValue Value, string Label)` is a record in
`CretNet.Platform.Blazor.Models`.

The component reads the current value via the compiled `Field`
expression and toggles by appending / removing from the array. A
mutation calls `State.Mutate(q => RecordCloner.With(q, propertyName, newArray))`,
which clones the record and reassigns the property via a cached
compiled setter.

### When you also want to reset paging on a filter change

The component itself only mutates the named field. Reset to page 1
in the page's filter handler if you need it — for example by
wrapping the helper with a `<FilterControls>` content that also
calls `_queryState.Mutate(q => q with { PageIndex = 1 })` after
toggle. In practice the paging mutator on `AttachQueryState` reads
the latest query and the next user-driven page click sees the
updated filter, so this only matters if you care about the very
next refetch coming back to page 1 immediately.

## Search

There's no `<CnpBindSearch>` component — the toolbar's
`<FluentSearch>` (built into `<CnpEntityDataGrid>`) already wires
end-to-end. Type → `OnSearchChanged` (300ms debounce) →
`LoadPageAsync` → in BackedBy mode, the `pagingMutator` you passed
to `AttachQueryState` folds the search term into the query.

## Sort

There's no `<CnpBindSort>` component either. FluentDataGrid emits
sort changes through its `RefreshItems` callback;
`<CnpEntityDataGrid>` intercepts that and calls
`DataSource.UpdateSort(SortSpec)`, which invokes the `sortMutator`
you passed to `AttachQueryState`.

### Sort allow-list

The allow-list lives **on the server** so a UI-supplied string can
never drive arbitrary EF property access. One static class per
query type:

```csharp
// WAM.Application/Tasks/Sorting/MyTasksSorting.cs
public static class MyTasksSorting
{
    public static readonly IReadOnlyDictionary<string, Action<ISpecificationBuilder<Domain.Task>, bool>> Fields =
        new Dictionary<string, Action<ISpecificationBuilder<Domain.Task>, bool>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"]            = (q, asc) => { if (asc) q.OrderBy(t => t.Name);              else q.OrderByDescending(t => t.Name); },
            ["Status"]          = (q, asc) => { if (asc) q.OrderBy(t => t.Status);            else q.OrderByDescending(t => t.Status); },
            ["ProjectName"]     = (q, asc) => { if (asc) q.OrderBy(t => t.Project!.Name);     else q.OrderByDescending(t => t.Project!.Name); },
            ["ProjectItemName"] = (q, asc) => { if (asc) q.OrderBy(t => t.ProjectItem!.Name); else q.OrderByDescending(t => t.ProjectItem!.Name); },
        };
}
```

In the spec, `Query.ApplySort(query.Sort, MyTasksSorting.Fields)`
validates the field and applies the ordering; unknown fields throw
`UnknownSortFieldException`, which the handler turns into a 400
BadRequest.

The key in the dictionary must match what
`PropertyColumn.Property` evaluates to on the WASM grid (e.g.
`x => x.Name` → `"Name"`). For sort-by-enum-with-localized-display,
use a `<TemplateColumn>` with explicit
`SortBy="@(GridSort<TRow>.ByAscending(x => x.Status))"` — see WAM's
`MyTasksGrid` for the pattern.

## What's deliberately not built

| Helper                  | Why not                                                                          |
| :---                    | :---                                                                             |
| `<CnpBindCheckbox>`     | Single boolean is one line of FluentCheckbox + a `Mutate` call — no value in a wrapper. |
| `<CnpBindSearch>`       | Toolbar `<FluentSearch>` already wires through `pagingMutator`.                  |
| `<CnpBindSort>`         | FluentDataGrid + `sortMutator` already wires through.                            |
| `<CnpBindRadioGroup>`   | Add when a real screen needs it — same shape as `CnpBindCheckboxGroup` but for `TValue?` not `TValue[]?`. |
| `<CnpBindDateRange>`    | Same — wait for a real use case, then either add a helper or just write inline. |

The pattern is reusable: copy `CnpBindCheckboxGroup.razor`, swap
the array semantics for whatever you need. `RecordCloner.With(...)`
handles the mutation for any property type.
