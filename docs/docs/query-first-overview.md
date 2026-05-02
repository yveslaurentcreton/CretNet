# Query-first data architecture

CretNet's `EntityDefinition` historically wired grids to data via two
imperative client-side hooks: `WithFetchPagedAction(...)` for the
fetch and `WithEntityFilter(...)` lambdas for filtering. That works
when paging is in-memory; it falls apart silently in
**server-paged** mode because the client-side filter lambdas can't
travel over the network — they just don't run, while the UI still
shows the filter checkboxes.

Query-first replaces the imperative wiring with a typed query.
Each user-facing screen is backed by a record that carries every
piece of mutable state — filters, search, sort, paging — as
first-class fields. The same record flows from the UI through the
data source through the API endpoint to the SQL handler. There is
exactly one source of truth.

## The four primitives

All four live in CretNet (no consumer needs to invent them):

| Type                          | Lives in                     | Role                                                                                |
| :---                          | :---                         | :---                                                                                |
| `IPagedQuery<TRow>`           | `CretNet.Platform.Querying`  | Marker interface every typed query implements (`PageIndex`, `PageSize`, `Search`, `Sort`). |
| `PagedResult<TRow>`           | `CretNet.Platform.Querying`  | Wire-shape result: `Items`, `TotalCount`, `PageIndex`, `PageSize`.                  |
| `QueryState<TQuery>`          | `CretNet.Platform.Blazor.Models` | Per-screen mutable wrapper around an immutable query record. UI mutates this; the data source observes it. |
| `EntityDefinition.BackedBy<TQuery, TAction>(...)` | `CretNet.Platform.Blazor.Services` | Slot on the entity definition that binds the data source to a typed query.          |

## When to use BackedBy vs the legacy `WithFetchPagedAction`

Use **BackedBy** when:

- You need server-side filtering (`Status IN (...)`) or sort, not just paging.
- The screen is one specific use case ("My Tasks", "Active Projects",
  "Customer Picker") — query-first encourages one definition per use case.
- You want the filter / search / sort / paging UI to actually drive the
  server in a single, observable, cancellable pipeline.

Stick with the legacy `WithFetchPagedAction` when:

- The data source is already shipped, working, and not on a screen
  with filter UX. (Migrate at leisure.)
- Filtering really is in-memory (small fixed list, no server round-trip).

## The three other options we considered

Worth knowing about so you don't propose them later:

- **Lambdas client-side (status quo)**. Silent bypass in server-paged
  mode. The bug class behind WAM's BUG-001.
- **OData / `Expression<Func<T, bool>>` over the wire**. Looks great
  in demos. In production it ships every database column as part of
  the public API, lets clients write unindexed scans, and makes
  schema changes break random clients. Pass.
- **One generic `/query` endpoint**. Same problem as OData with a
  thinner veneer. The clever-feeling endpoint count is a trap; we'd
  trade one screen-specific endpoint per screen (which is fine —
  Stripe, GitHub, internal Google APIs all do this) for one mega-
  endpoint that accumulates branches.

We deliberately picked screen-specific typed queries with a
consistent envelope (`IPagedQuery<TRow>` + `PagedResult<TRow>`) over
all of these.

## Anatomy of a BackedBy screen

End-to-end shape (see [Quickstart](query-first-quickstart.md) for a
working walkthrough; see WAM's `MyTasksQuery` for a live reference):

```
WAM.Application
  ├─ Tasks/Queries/MyTasksQuery.cs           ← record IPagedQuery<MyTaskRow>, IRequest<...>
  ├─ Tasks/Rows/MyTaskRow.cs                 ← projection record
  ├─ Tasks/Specifications/MyTasksSpecification.cs  ← Ardalis spec, composed
  ├─ Tasks/Specifications/TaskSpecificationExtensions.cs ← reusable WHERE-builders
  ├─ Tasks/Sorting/MyTasksSorting.cs         ← allow-list of sortable fields
  └─ Tasks/Handlers/MyTasksQueryHandler.cs   ← MediatR handler

WAM.WebApi
  ├─ Tasks/Endpoints/MyTasks.cs              ← FastEndpoints endpoint, GET /me/tasks
  ├─ Tasks/Endpoints/MyTasks.Request.cs      ← wire DTO
  ├─ Tasks/Endpoints/MyTasks.Response.cs     ← wire DTO
  ├─ Tasks/Dtos/MyTaskRowDto.cs              ← row DTO
  └─ Tasks/Mappings/TaskMapper.cs            ← Mapperly: request→query, paged→response

WAM.Wasm
  ├─ Tasks/Models/MyTaskItem.cs              ← WASM-side row (IIdentity<TId>)
  ├─ Tasks/Queries/MyTasksQuery.cs           ← WASM-side query record (mirrors server)
  ├─ Tasks/State/Actions/FetchMyTasks.cs     ← Fluxor action, HTTP call
  ├─ Tasks/Services/MyTasksDefinition.cs     ← EntityDefinition.BackedBy<MyTasksQuery, ...>
  ├─ Tasks/Components/MyTasksGrid.razor      ← grid component (forwards FilterControls)
  └─ General/Pages/MyTasks.razor + .cs       ← page: builds QueryState, renders bindings
```

Each piece is small. Most files are 20–50 lines. The cross-cutting
concerns (envelope, paging, search, sort, cancellation, error
handling, empty state) live once in CretNet.

## What ships in CretNet for this

- **Wire envelope** — `IPagedQuery<TRow>`, `PagedResult<TRow>`, `SortSpec`, `SortDirection`.
- **Per-screen state** — `QueryState<TQuery>` (BehaviorSubject under the hood).
- **Definition slot** — `EntityDefinition<TEntity, TId>.BackedBy<TQuery, TAction>(...)`.
- **Data source** — `CnpDataSource<TEntity, TId>` natively understands BackedBy:
  - `AttachQueryState(state, pagingMutator?, sortMutator?)` wires UI ↔ query state.
  - `Reload()` in BackedBy mode re-fetches the current `QueryState.Current`.
  - Last-write-wins fetches via Rx `Switch` — fast user clicks cancel in-flight requests.
  - End-to-end `CancellationToken` (Rx side; Fluxor side is a future improvement).
  - `LastError` exposed when a fetch fails; toast emitted via `ICnpToastService`.
- **Grid** — `CnpEntityDataGrid<TGridItem, TId>`:
  - `<FilterControls>` slot for `<CnpBindCheckboxGroup>` and friends inside the filter-button popover.
  - `<EmptyContent>` slot rendered when a successful fetch returned no rows.
  - `<ErrorContent>` slot rendered when `DataSource.LastError` is set; default has a Retry button.
  - Column-header sort routed through `request.GetSortByProperties()` → `DataSource.UpdateSort(...)` → query-state mutation → fetch.
  - Pagination + search wired through `pagingMutator`; sort through `sortMutator`.
- **Binding helper** — `<CnpBindCheckboxGroup TQuery TValue>` for multi-value filters,
  with compile-time field-name safety (rename → break the build) via `RecordCloner`.

## Next

- Build your first BackedBy screen — see the [Quickstart](query-first-quickstart.md).
- Reference for binding components — see [Bindings](query-first-bindings.md).
- Migrating an existing legacy screen — see [Migration](query-first-migration.md).
