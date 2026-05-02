# Query-first quickstart

This walks through building a BackedBy screen end-to-end, mirroring
WAM's My Tasks (`/mytasks`). The example assumes:

- A `Domain.Task` entity (anything `IEntity<Guid>` works).
- A repository registered as `IRepository<Domain.Task, Guid>`.
- The standard CretNet WASM stack (Fluxor, FluentUI, MediatR).

It takes ~30 minutes the first time, ~10 minutes the second time.
For a live reference, read WAM's `MyTasksQuery` /
`MyTasksQueryHandler` / `MyTasks.razor`.

## 1. Define the query record

Lives in your application layer. Implements `IPagedQuery<TRow>`
(for the envelope) and your mediator's request marker.

```csharp
// WAM.Application/Tasks/Queries/MyTasksQuery.cs
public sealed record MyTasksQuery(
    Guid WorkerId,
    TaskStatus[]? IncludeStatuses = null,
    string? Search = null,
    int PageIndex = 1,
    int PageSize = 20,
    SortSpec? Sort = null
) : IPagedQuery<MyTaskRow>, IRequest<Result<PagedResult<MyTaskRow>>>;
```

The record-with-init-only-properties shape is exactly what
`QueryState<MyTasksQuery>.Mutate(q => q with { ... })` needs.

## 2. Define the row projection

Plain record, only the fields the grid actually renders. Don't
include domain entity references — that's what the projection is for.

```csharp
// WAM.Application/Tasks/Rows/MyTaskRow.cs
public sealed record MyTaskRow
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required TaskStatus Status { get; init; }
    public string? ProjectName { get; init; }
    public bool IsTimerActive { get; init; }
    public string DisplayDuration { get; init; } = "00:00:00";
}
```

## 3. Write the specification

One per use case, composing reusable extension methods on
`ISpecificationBuilder<T>`.

```csharp
// WAM.Application/Tasks/Specifications/TaskSpecificationExtensions.cs
public static class TaskSpecificationExtensions
{
    public static ISpecificationBuilder<Domain.Task> ForWorker(
        this ISpecificationBuilder<Domain.Task> q, Guid workerId)
    {
        q.Where(t => t.AssignedToPartyId == workerId);
        return q;
    }

    public static ISpecificationBuilder<Domain.Task> WithStatuses(
        this ISpecificationBuilder<Domain.Task> q, TaskStatus[]? statuses)
    {
        if (statuses is { Length: > 0 })
            q.Where(t => statuses.Contains(t.Status));
        return q;
    }
}

// WAM.Application/Tasks/Specifications/MyTasksSpecification.cs
public sealed class MyTasksSpecification : Specification<Domain.Task>
{
    public MyTasksSpecification(MyTasksQuery query)
    {
        Query
            .ForWorker(query.WorkerId)
            .WithStatuses(query.IncludeStatuses);

        Query.Include(t => t.Project);
        Query.OrderBy(t => t.Name);   // default sort
    }
}
```

> **Search lives in Infrastructure.** Free-text search uses provider-
> specific functions like `EF.Functions.ILike` and shouldn't leak into
> the Application layer. Implement `EntitySearchSpecification<Task>`
> in `WAM.Infrastructure/Tasks/Specifications/`; the repository auto-
> applies it when `PagingOptions.Search` is set. See WAM's
> `TaskSearchSpecification` for the pattern.

## 4. Write the handler

```csharp
// WAM.Application/Tasks/Handlers/MyTasksQueryHandler.cs
public sealed class MyTasksQueryHandler
    : IRequestHandler<MyTasksQuery, Result<PagedResult<MyTaskRow>>>
{
    private readonly IRepository<Domain.Task, Guid> _repo;
    public MyTasksQueryHandler(IRepository<Domain.Task, Guid> repo) => _repo = repo;

    public async Task<Result<PagedResult<MyTaskRow>>> Handle(MyTasksQuery query, CancellationToken ct)
    {
        var spec = new MyTasksSpecification(query);

        var paged = await _repo.GetAllAsync(
            new PagingOptions
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Search = query.Search,
            },
            spec, cancellationToken: ct);

        var rows = paged.Items.Select(MapRow).ToList();

        return new PagedResult<MyTaskRow>
        {
            Items = rows,
            TotalCount = paged.TotalCount,
            PageIndex = paged.PageIndex,
            PageSize = paged.PageSize,
        };
    }

    private static MyTaskRow MapRow(Domain.Task t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Status = t.Status,
        ProjectName = t.Project?.DisplayName,
        IsTimerActive = t.TimeEntries.Any(te => te.EndTime is null),
    };
}
```

## 5. Add the API endpoint

```csharp
// WAM.WebApi/Tasks/Endpoints/MyTasks.Request.cs
public class MyTasksRequest
{
    public required Guid WorkerId { get; set; }
    public List<TaskStatus>? IncludeStatuses { get; set; }
    public string? Search { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public SortDirection? SortDirection { get; set; }
}

// WAM.WebApi/Tasks/Endpoints/MyTasks.Response.cs
public class MyTasksDtoResponse
{
    public required IReadOnlyList<MyTaskRowDto> Items { get; set; }
    public required int TotalCount { get; set; }
    public required int PageIndex { get; set; }
    public required int PageSize { get; set; }
}

// WAM.WebApi/Tasks/Endpoints/MyTasks.cs
public class MyTasksEndpoint : Ep.Req<MyTasksRequest>.Res<...>
{
    public override void Configure() => Get("/me/tasks");
    public override async Task<...> ExecuteAsync(MyTasksRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.ToMyTasksQuery(), ct);
        return result.Match(paged => Ok(paged.ToDto()), ex => BadRequest(ex));
    }
}
```

Sort splits across two query parameters (`SortField`, `SortDirection`)
because FastEndpoints' query binder doesn't bind nested complex types.

## 6. WASM-side row, query, action

The WASM mirrors the server-side query record (contract test in S-083
will enforce shape parity).

```csharp
// WAM.Wasm/Tasks/Models/MyTaskItem.cs
public class MyTaskItem : IIdentity<Guid>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public TaskStatus Status { get; set; }
    public string? ProjectName { get; set; }
    public bool IsTimerActive { get; set; }
    public string DisplayDuration { get; set; } = "00:00:00";
    public string DisplayName => string.IsNullOrWhiteSpace(ProjectName) ? Name : $"{Name} ({ProjectName})";
}

// WAM.Wasm/Tasks/Queries/MyTasksQuery.cs
public sealed record MyTasksQuery(
    Guid WorkerId,
    TaskStatus[]? IncludeStatuses = null,
    string? Search = null,
    int PageIndex = 1,
    int PageSize = 20,
    SortSpec? Sort = null
) : IPagedQuery<MyTaskItem>;

// WAM.Wasm/Tasks/State/Actions/FetchMyTasks.cs
[CnpEntityAction(typeof(PagedResult<MyTaskItem>))]
public partial class FetchMyTasksAction(MyTasksQuery Query)
{
    [CnpInject] public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    public async Task<PagedResult<MyTaskItem>> Effect(IDispatcher d)
    {
        var http = HttpClientFactory.CreateClient("WamWebApi");
        var response = await http.GetFromJsonAsync<WireResponse>(BuildUrl(Query));
        // ... map to PagedResult<MyTaskItem>
    }
}
```

## 7. Entity definition

```csharp
// WAM.Wasm/Tasks/Services/MyTasksDefinition.cs
public class MyTasksDefinition : EntityDefinition<MyTaskItem, Guid>
{
    public MyTasksDefinition() => Entity()
        .WithLabel("Task").WithPluralLabel("Tasks")
        .WithIdentifier(x => x.DisplayName)
        .WithDisplayName(x => x.DisplayName)
        .WithDataGrid<MyTasksGrid>()
        .WithNavigationAction<NavigateToMyTaskAction>(id => new NavigateToMyTaskAction(id))
        .BackedBy<MyTasksQuery, FetchMyTasksAction>(query => new FetchMyTasksAction(query));
}
```

Register it like any other entity definition (see WAM's
`AddEntityDefinitions` extension).

## 8. Page and grid

```razor
@* MyTasks.razor *@
@page "/mytasks"

<MyTasksGrid AfterInit="AttachQueryState">
    <FilterControls>
        <CnpBindCheckboxGroup TQuery="MyTasksQuery" TValue="TaskStatus"
                              State="_queryState!"
                              Field="q => q.IncludeStatuses"
                              Category="Status"
                              Options="_statusOptions" />
    </FilterControls>
</MyTasksGrid>

@code {
    private static readonly IReadOnlyList<CheckboxOption<TaskStatus>> _statusOptions =
    [
        new(TaskStatus.ToDo,       "To Do"),
        new(TaskStatus.InProgress, "In Progress"),
        new(TaskStatus.OnHold,     "On Hold"),
        new(TaskStatus.Completed,  "Completed"),
    ];
}
```

```csharp
// MyTasks.razor.cs
public partial class MyTasks : IDisposable
{
    private QueryState<MyTasksQuery>? _queryState;

    protected override async Task OnInitializedAsync()
    {
        var worker = await Dispatcher.DispatchAsync(new FetchLoggedInWorkerAction());
        _queryState = new(new MyTasksQuery(
            WorkerId: worker.Party.Id,
            IncludeStatuses: [TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.OnHold]));
    }

    private Task AttachQueryState(ICnpDataSource<MyTaskItem, Guid> ds)
    {
        ds.AttachQueryState(
            _queryState!,
            pagingMutator: (q, page, size, search) =>
                q with { PageIndex = page, PageSize = size, Search = search },
            sortMutator: (q, sort) => q with { Sort = sort, PageIndex = 1 });
        return Task.CompletedTask;
    }

    public void Dispose() => _queryState?.Dispose();
}
```

The grid component is a thin wrapper that forwards `AfterInit` and
`FilterControls` to `<CnpEntityDataGrid>` and declares the columns —
see WAM's `MyTasksGrid.razor` for the standard column layout.

## What you get for free

After step 8, your screen has:

- **Working filters** — toggling a checkbox in the popover mutates
  `QueryState`, the data source refetches with the new
  `IncludeStatuses`, the paginator's `TotalCount` reflects the
  filtered total.
- **Working search** — typing in the toolbar search box debounces
  300ms, then refetches.
- **Working sort** — clicking a column header refetches with the new
  `SortSpec`. Add columns to your sortable allow-list (see [Bindings](query-first-bindings.md#sort-allow-list)).
- **Working pagination** — clicking a page number refetches.
- **Cancellation** — fast clicks cancel in-flight fetches via
  Rx `Switch`.
- **Error handling** — a fetch failure shows an inline Retry banner;
  the previous rows stay visible so you keep context.
- **Empty state** — zero results render a "No results match your filters." panel.
- **Reload button** — the toolbar's Refresh button re-fetches the current state.

If you don't want any of those for free, override `<EmptyContent>`
or `<ErrorContent>` on `<CnpEntityDataGrid>`, or omit the
`pagingMutator` / `sortMutator` to make those interactions no-ops.
