# Migrating a legacy `WithFetchPagedAction` screen to BackedBy

Side-by-side guide for converting an existing legacy server-paged
screen onto query-first. WAM's My Tasks (`/mytasks`) is the
reference: see commits `feat(M-009/S-076): pilot My Tasks on
query-first architecture` and the follow-up fixes for the full
diff.

## Before — legacy

```csharp
// WAM.Wasm/Tasks/Services/TaskDefinition.cs
public class TaskDefinition : EntityDefinition<WamTask, Guid>
{
    public TaskDefinition() => Entity()
        .WithDataGrid<TaskDataGrid>()
        .WithFetchPagedAction<TaskDependencyArgs, FetchTasksPagedAction>(
            (args, page, size, search) =>
                new FetchTasksPagedAction(page, size, search, args.AssignedToPartyId, args.ProjectId))
        .WithEntityFilter(Labels.Status, 1, Labels.ToDo,       true,  q => q.Where(x => x.Status == TaskStatus.ToDo))
        .WithEntityFilter(Labels.Status, 2, Labels.InProgress, true,  q => q.Where(x => x.Status == TaskStatus.InProgress))
        .WithEntityFilter(Labels.Status, 3, Labels.OnHold,     true,  q => q.Where(x => x.Status == TaskStatus.OnHold))
        .WithEntityFilter(Labels.Status, 4, Labels.Completed,  false, q => q.Where(x => x.Status == TaskStatus.Completed))
        // ... CRUD action wiring
}
```

The `WithEntityFilter` lambdas look like they filter, but
`CnpDataSource` skips them in server-paged mode (consistency with
the server-returned `TotalCount`). UI bug: the Status checkboxes do
nothing.

## After — BackedBy

The equivalent screen lives in **its own definition** alongside the
legacy `TaskDefinition` (which keeps serving non-MyTasks contexts
until they migrate too). The two definitions use different `TEntity`
types so DI doesn't conflict.

```csharp
// WAM.Wasm/Tasks/Services/MyTasksDefinition.cs
public class MyTasksDefinition : EntityDefinition<MyTaskItem, Guid>
{
    public MyTasksDefinition() => Entity()
        .WithDataGrid<MyTasksGrid>()
        .WithNavigationAction<NavigateToMyTaskAction>(id => new NavigateToMyTaskAction(id))
        .BackedBy<MyTasksQuery, FetchMyTasksAction>(query => new FetchMyTasksAction(query));
    // No WithEntityFilter calls — filter UI lives in the page via <CnpBindCheckboxGroup>.
}
```

The eleven other moving pieces (query record, row, spec, handler,
endpoint, mapper, action, page, grid wrapper, sorting registry,
search spec) are documented in the [Quickstart](query-first-quickstart.md).
This page focuses on what's different vs the legacy.

## Diff at a glance

| Concern                | Legacy                                                | BackedBy                                                                  |
| :---                   | :---                                                  | :---                                                                      |
| Definition shape       | `EntityDefinition<TEntity, TId>` shared across all uses of the entity | `EntityDefinition<TRow, TId>` per screen / use case (one definition per query) |
| Fetch contract         | `(args, page, size, search) => action`                | `(typed query) => action`                                                 |
| Filter UI              | `WithEntityFilter(...)` — silently bypassed server-side | `<CnpBindCheckboxGroup>` inside the `<FilterControls>` slot              |
| Search                 | Wired via `DataSource.Filter`                         | Same — works through `pagingMutator`                                      |
| Sort                   | Client-side per-page (FluentDataGrid in-memory)       | Server-side via `sortMutator` and the per-query allow-list               |
| Failure handling       | Logged, items blanked                                  | Toast + Retry banner via `<ErrorContent>`, items preserved                |
| Empty result           | Blank grid                                            | `<EmptyContent>` ("No results match your filters.")                       |
| Reload button          | Calls legacy `LoadData()` — no-op for paged           | Re-fetches current `QueryState`                                           |
| Search spec            | Same `EntitySearchSpecification<T>` in Infrastructure | Same — auto-applied via `PagingOptions.Search`                            |
| Free-text search       | `WithFilter((text, x) => ...)` lambda                  | Server-side via the existing search spec                                  |

## Mechanical migration steps

For each legacy screen:

1. **Pick the row shape**. If the screen renders a subset of the
   entity, define a slim `Row` (`IIdentity<TId>` for the WASM side).
   Otherwise reuse the existing model.
2. **Define the typed query** in the application layer. Add fields
   for every UI-driven filter; the WASM mirrors the same record.
3. **Move the filter logic to a spec** (or extension methods on
   `ISpecificationBuilder<T>`). Move free-text search to an
   `EntitySearchSpecification<T>` in Infrastructure if you don't
   already have one.
4. **Add the sort allow-list** (`{Entity}Sorting.Fields`) — keys
   must match the WASM `PropertyColumn.Property` names.
5. **Add the endpoint + DTOs + Mapperly mappings**. Follow the
   shape of the existing `GetAll{Entity}` endpoint; you can ship
   the new endpoint alongside the legacy one and migrate consumers.
6. **Add the WASM-side query, row model, fetch action, definition,
   grid component**. Keep the page minimal — most logic moved into
   the spec / handler.
7. **Register the new `EntityDefinition<NewRow, TId>`** in
   `AddEntityDefinitions`. Don't remove the legacy one yet — other
   screens still use it.
8. **Rewrite the page** to use `QueryState`, `AttachQueryState`,
   and `<CnpBindCheckboxGroup>` for filters.
9. **Verify in browser**: filter / search / sort / paging all
   trigger one network call each; combinations compose.
10. **Once every screen using the entity is migrated**, remove the
    legacy definition and the `WithFetchPagedAction` overload usage.

## CRUD parity via adapter actions

The legacy `EntityDefinition` shared its `WithOpenAddDialogAction` /
`WithOpenEditDialogAction` / `WithOpenRemoveDialogAction` wiring with
the legacy data grid because the data source's `TEntity` matched what
the dialog actions accepted. In BackedBy mode the data source is typed
on a slim row projection (e.g. `ProjectListItem`), but the legacy
dialog actions still take the full domain entity (`Project`). Two
pieces close that gap:

### 1. Per-op adapter actions

Naming convention: `Open{Op}{Entity}FromListDialogAction`. Three
flavours:

```csharp
// Add — takes context, dispatches the legacy add dialog,
//       maps the created entity to the row.
[CnpEntityAction(typeof(ProjectListItem), false, true)]
public partial class OpenAddProjectFromListDialogAction(Guid? CustomerId, Guid? WorkerId)
{
    public async Task<ProjectListItem?> Effect(IDispatcher dispatcher)
    {
        var created = await dispatcher.DispatchAsync(new OpenAddProjectDialogAction(CustomerId, WorkerId));
        return created is null ? null : MapToListItem(created);
    }
}

// Edit / Remove — fetch the full entity by id first, then dispatch.
[CnpEntityAction(typeof(ProjectListItem), false, true)]
public partial class OpenEditProjectFromListDialogAction(ProjectListItem ListItem)
{
    public async Task<ProjectListItem?> Effect(IDispatcher dispatcher)
    {
        var project = await dispatcher.DispatchAsync(new FetchProjectAction(ListItem.Id));
        if (project is null) return null;
        var updated = await dispatcher.DispatchAsync(new OpenEditProjectDialogAction(project));
        return updated is null ? null : MapToListItem(updated);
    }
}
```

Wire on the per-screen `XListDefinition` via the standard fluent
methods:

```csharp
.WithOpenAddDialogAction<ProjectListDependencyArgs, OpenAddProjectFromListDialogAction>(
    args => new OpenAddProjectFromListDialogAction(args.CustomerId, args.WorkerId))
.WithOpenEditDialogAction(item => new OpenEditProjectFromListDialogAction(item))
.WithOpenRemoveDialogAction(item => new OpenRemoveProjectFromListDialogAction(item));
```

The page-context args (`XListDependencyArgs` with `CustomerId`,
`WorkerId`, etc.) flow through the grid component's
`DependencyArgs` callback into the Add adapter.

### 2. Auto-reload after Add / Edit / Remove

Legacy data sources keep their cache in step with CRUD via the
`SubscribeCreateSuccess` / `SubscribeUpdateSuccess` /
`SubscribeDeleteSuccess` events the dispatched action fires —
that works because the entity types line up. BackedBy adapter
actions wrap a legacy dialog action whose entity type differs
from the data source's `TEntity`, so those subscribe-success
hooks fire for the **wrapped** entity (e.g. `Project`) and never
reach the data source's `TEntity` (`ProjectListItem`).

CretNet's `CnpDataSource.Add` / `Edit` / `Remove` therefore
**auto-reload** the current `QueryState` after the underlying
dispatch completes (in BackedBy mode). One extra round trip per
CRUD op; cheap, reliable, and keeps the adapter actions trivial.

The same auto-reload also covers bulk actions — see "Bulk
SecondaryActions" below.

## Bulk SecondaryActions parity

Same adapter pattern as per-row CRUD, taking `IEnumerable<TRow>`
instead of one row. The page-side `<SecondaryActions>` block
dispatches the bulk adapter, which fetches the entities by id
(if needed) and dispatches the legacy bulk dialog action.

```csharp
[CnpEntityAction(typeof(ProjectListItem), false, true)]
public partial class OpenChangeProjectStatusFromListDialogAction(IEnumerable<Guid> Ids)
{
    public async Task<ProjectListItem?> Effect(IDispatcher dispatcher)
    {
        var dispatched = await dispatcher.DispatchAsync(new OpenChangeProjectStatusDialogAction(Ids));
        return null;  // auto-reload picks up the changes
    }
}
```

Render in the grid's `<SecondaryActions>` slot, same as the legacy
grid did.

## Gotchas

- **TEntity vs TRow**. The legacy `WamTask` has client-computed
  properties (`IsTimerActive`, `DisplayDuration`) that depend on
  related TimeEntries. The BackedBy row should compute these
  server-side and ship them as plain fields — single source of
  truth, smaller payload.
- **Search spec lives in Infrastructure**, not Application. EF
  provider-specific functions (`EF.Functions.ILike`) leak otherwise.
- **CRUD actions on the new definition.** `EntityDefinition.WithNavigationAction<TAction>(...)`
  wants `TAction : ICnpEntityAction<TRow?>`. If your existing
  navigation/edit/etc. actions return the legacy entity type, you
  need parallel actions for the new row type — see WAM's
  `NavigateToMyTaskAction`. Add / Edit / Remove dialogs are the same
  story; defer them to a follow-up if your pilot doesn't need them.
- **`PropertyColumn` with a localizer-lookup expression won't
  surface a sortable property name.** Use `<TemplateColumn>` with
  explicit `SortBy="@(GridSort<TRow>.ByAscending(x => x.Field))"`.
- **`CustomFilterFunc` is mutually exclusive with BackedBy.** If
  you set both, `Init()` throws — fail-fast on misuse.
- **The submodule (CretNet) and the consumer repo (WAM) move
  together.** Make CretNet changes on a branch, bump the submodule
  pointer in the consumer commit. Push the CretNet branch before
  CI / fresh clones see the consumer commit.

## What you don't have to migrate

- `IsServerPaged` paths in CretNet stay supported — legacy
  `WithFetchPagedAction` consumers keep working.
- `WithEntityFilter` is allowed in BackedBy mode (renders in the
  filter popover) but bypassed for filtering. Remove on migration
  for clarity; not required.
- Other entities you're not migrating yet. The two architectures
  coexist cleanly per definition.
