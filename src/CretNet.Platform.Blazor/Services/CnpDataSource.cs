using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using Fluxor;
using CretNet.Platform.Blazor.Models;
using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using CretNet.Platform.Querying;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using SortDirection = DynamicData.Binding.SortDirection;

namespace CretNet.Platform.Blazor.Services
{
    public interface ICnpDataSource<TEntity, TId> : IDisposable
        where TEntity : IIdentity<TId>
        where TId : notnull
    {
        ReadOnlyObservableCollection<TEntity>? Entities { get; }
        ReadOnlyObservableCollection<TEntity>? SelectedEntities { get; }
        bool IsLoading { get; }

        /// <summary>
        /// True when the associated <c>IEntityDefinition</c> provides a paged fetch action.
        /// In this mode the server is the single source of truth for paging, searching and
        /// filtering: client-side <c>CustomFilterFunc</c>, <c>EntityFilters</c> and text
        /// filtering are bypassed because they would be inconsistent with the
        /// server-returned <see cref="TotalCount"/>. Entity definitions configured for
        /// server paging must perform all filtering on the server side.
        /// </summary>
        bool IsServerPaged { get; }

        /// <summary>
        /// True when the entity definition is bound to a typed
        /// <c>IPagedQuery&lt;TEntity&gt;</c> via <c>BackedBy&lt;TQuery&gt;</c>.
        /// In this mode the page must call <see cref="AttachQueryState{TQuery}"/>
        /// once during initialisation; the data source then refetches on every
        /// query change. <see cref="EntityFilters"/> are kept for the visual
        /// filter-button popover (the page is responsible for syncing their
        /// Enabled state into the query). <see cref="CustomFilterFunc"/> remains
        /// mutually exclusive with this mode — it has no visual representation
        /// and would silently do nothing.
        /// </summary>
        bool IsBackedByQuery { get; }

        /// <summary>
        /// Attach the per-screen query state. The data source subscribes to
        /// <see cref="QueryState{TQuery}.Changes"/> and refetches on every
        /// emission. Throws if the entity definition is not configured with
        /// <c>BackedBy&lt;TQuery&gt;</c>.
        /// </summary>
        /// <param name="queryState">The mutable wrapper around the typed query.</param>
        /// <param name="pagingMutator">
        /// Optional callback the data source uses to fold paginator and search-box
        /// changes back into the query — typically
        /// <c>(q, page, size, search) =&gt; q with { PageIndex = page, PageSize = size, Search = search }</c>.
        /// Without it, page-clicks and the search box are no-ops in BackedBy
        /// mode (filters still work, since they go through your own mutations).
        /// </param>
        void AttachQueryState<TQuery>(
            QueryState<TQuery> queryState,
            Func<TQuery, int, int, string?, TQuery>? pagingMutator = null
        ) where TQuery : class;

        int TotalCount { get; }
        Action? OnStateHasChanged { get; set; }
        Task Init();
        Task<TEntity?> Add();
        Task Edit(TEntity entity);
        Task Remove(IEnumerable<TEntity> entities);
        void Navigate(TEntity entity);
        string Filter { get; set; }
        Func<object>? DependencyArgs { get; set; }
        Func<TEntity, bool>? CustomFilterFunc { get; set; }
        bool CanNavigate { get; }
        bool CanAdd { get; }
        bool CanEdit { get; }
        bool CanRemove { get; }
        bool MultiSelect { get; set; }
        Action<IEnumerable<TEntity>>? SelectedEntitiesChanged { get; set; }
        Action? SelectedEntitiesCleared { get; set; }
        void Refresh();
        Task Reload();
        Task LoadPageAsync(int pageIndex, int pageSize, string? search = null);
        void SelectItem(TEntity entity);
        void DeselectItem(TEntity? entity);
        bool IsSelected(TEntity entity);
        bool? IsAllSelected();
        ObservableCollection<EntityFilter<TEntity>> EntityFilters { get; }
        EntityFilterType EntityFilterType { get; set; }
    }

    public class CnpDataSource<TEntity, TId> : ICnpDataSource<TEntity, TId>
        where TEntity : IIdentity<TId>
        where TId : notnull
    {
        public bool IsLoading { get; private set; } = true;
        public bool IsServerPaged => _entityDefinition?.HasFetchPagedAction == true;
        public bool IsBackedByQuery => _entityDefinition?.HasBackedByQuery == true;
        public int TotalCount { get; private set; }

        private readonly IActionSubscriber _actionSubscriber;
        private readonly IDispatcher _dispatcher;
        private readonly IEntityDefinition<TEntity, TId>? _entityDefinition;
        private readonly ILogger<CnpDataSource<TEntity, TId>>? _logger;

        private readonly CompositeDisposable _garbage = new();

        protected readonly SourceCache<TEntity, TId> _entityCache = new(entity => entity.Id);
        protected ReadOnlyObservableCollection<TEntity>? _entities;
        public ReadOnlyObservableCollection<TEntity>? Entities => _entities;
        
        protected readonly SourceCache<TEntity, TId> _selectedEntityCache = new(entity => entity.Id);
        protected ReadOnlyObservableCollection<TEntity>? _selectedEntities;
        public ReadOnlyObservableCollection<TEntity>? SelectedEntities => _selectedEntities;
        public Action<IEnumerable<TEntity>>? SelectedEntitiesChanged { get; set; }
        public Action? SelectedEntitiesCleared { get; set; }

        private readonly BehaviorSubject<string> _filterSubject = new(string.Empty);
        public string Filter
        {
            get => _filterSubject.Value;
            set => _filterSubject.OnNext(value);
        }
        public Func<TEntity, bool>? CustomFilterFunc { get; set; }
        public Func<object>? DependencyArgs { get; set; }
        
        public ObservableCollection<EntityFilter<TEntity>> EntityFilters { get; } = new();
        public EntityFilterType EntityFilterType { get; set; } = EntityFilterType.Default;
        public bool MultiSelect { get; set; }
        public bool CanNavigate => _entityDefinition?.HasNavigationAction == true;
        public bool CanAdd => _entityDefinition?.HasOpenAddDialogAction == true;
        public bool CanEdit => _entityDefinition?.HasOpenEditDialogAction == true;
        public bool CanRemove => _entityDefinition?.HasOpenRemoveDialogAction == true || _entityDefinition?.HasOpenRemoveMultipleDialogActionFactory == true;
        
        public Action? OnStateHasChanged { get; set; }
        
        public CnpDataSource(
            IServiceProvider serviceProvider,
            IActionSubscriber actionSubscriber,
            IDispatcher dispatcher)
        {
            _actionSubscriber = actionSubscriber;
            _dispatcher = dispatcher;
            _entityDefinition = serviceProvider.GetService<IEntityDefinition<TEntity, TId>>();
            _logger = serviceProvider.GetService<ILogger<CnpDataSource<TEntity, TId>>>();
        }

        public async Task Init()
        {
            EntityFilters.Clear();
            var enable = EntityFilterType == EntityFilterType.None;

            foreach (var filter in _entityDefinition?.GetEntityFilters()?.OfType<EntityFilter<TEntity>>() ??
                                   Enumerable.Empty<EntityFilter<TEntity>>())
            {
                EntityFilters.Add(filter);
                
                if (enable)
                    filter.Enabled = true;
            }

            // Dispose previous filter subscriptions if any
            _filtersSubscription?.Dispose();

            // Subscribe to Enabled changes for all filters
            var enabledChanged = EntityFilters
                .Select(f => f.WhenAnyValue(x => x.Enabled))
                .Merge();
            _filtersSubscription = enabledChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => {
                    _filterSubject.OnNext(_filterSubject.Value);
                    StateHasChanged();
                });

            _entityDefinition?.SubscribeCreateSuccess(_actionSubscriber, this, action =>
            {
                var entity = action.Entity;
                    _entityCache.AddOrUpdate(entity);
                _selectedEntityCache.AddOrUpdate(entity);
                StateHasChanged();
            });
            _entityDefinition?.SubscribeUpdateSuccess(_actionSubscriber, this, action =>
            {
                var entity = action.Entity;
                    _entityCache.AddOrUpdate(entity);
                _selectedEntityCache.AddOrUpdate(entity);
                StateHasChanged();
            });
            _entityDefinition?.SubscribeDeleteSuccess(_actionSubscriber, this, action =>
            {
                var entityId = action.Entity.Id;
                _entityCache.RemoveKey(entityId);
                _selectedEntityCache.RemoveKey(entityId);
                StateHasChanged();
            });
            _entityDefinition?.SubscribeRefresh(_actionSubscriber, this, () =>
            {
                Refresh();
                StateHasChanged();
            });
            _entityDefinition?.SubscribeReload(_actionSubscriber, this, () =>
            {
                Reload();
            });
            
            var defaultFilterFunc = _entityDefinition?.FilterFunc ?? ((_, _) => true);
            var sortFunc = _entityDefinition?.SortByFunc ?? ((_) => 0);
            var sortOrder = _entityDefinition?.SortOrder ?? SortDirection.Ascending;

            var entityFiltersChanged = EntityFilters
                .Select(f => f.WhenAnyValue(x => x.Enabled))
                .Merge()
                .Select(_ => _filterSubject.Value);

            var combinedFilterTrigger = Observable.Merge(
                _filterSubject,
                entityFiltersChanged
            );

            _entityCache
                .Connect()
                .Filter(combinedFilterTrigger.Select(text =>
                {
                    var enabledFilters = EntityFilters.Where(f => f.Enabled).ToList();
                    var grouped = EntityFilters.GroupBy(f => f.Category).ToList();

                    return new Func<TEntity, bool>(entity =>
                    {
                        // In server-paged or BackedBy mode the server is the single source of
                        // truth for filtering/paging; client-side filters would be inconsistent
                        // with the server-returned TotalCount, so we bypass them entirely.
                        if (IsServerPaged || IsBackedByQuery)
                            return true;

                        var customFilterResult = CustomFilterFunc?.Invoke(entity) != false;
                        if (!customFilterResult) return false;

                        var textFilterResult = string.IsNullOrWhiteSpace(text) || defaultFilterFunc(text, entity);
                        if (!textFilterResult) return false;

                        // Category-based filter logic
                        foreach (var category in grouped)
                        {
                            var filtersInCategory = category.ToList();
                            var enabledInCategory = filtersInCategory.Where(f => f.Enabled).ToList();
                            if (enabledInCategory.Count == 0)
                                return false; // none enabled: show none
                            if (enabledInCategory.Count == filtersInCategory.Count)
                                continue; // all enabled: don't filter on this category
                            // at least one enabled: entity must match at least one enabled filter in this category
                            var queryable = new[] { entity }.AsQueryable();
                            var anyMatch = enabledInCategory.Any(f => f.ApplyFilter(queryable).Any());
                            if (!anyMatch)
                                return false;
                        }
                        return true;
                    });
                }))
                .SortBy(sortFunc, sortOrder)
                .Bind(out _entities)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_garbage);
            
            _selectedEntityCache
                .Connect()
                .Bind(out _selectedEntities)
                .Subscribe(_ => SelectedEntitiesChanged?.Invoke(_selectedEntities))
                .DisposeWith(_garbage);

            // In BackedBy mode the page drives loads via AttachQueryState; nothing
            // happens here until that is called. EntityFilters are still allowed —
            // CretNet renders them in the filter-button popover for visual parity
            // with the legacy data sources, but the page is responsible for syncing
            // EntityFilter.Enabled changes back into the QueryState. CustomFilterFunc
            // on the other hand has no visual representation and would silently do
            // nothing, so we error on it.
            if (IsBackedByQuery)
            {
                if (CustomFilterFunc is not null)
                {
                    _logger?.LogError(
                        "CnpDataSource<{Entity}> is configured with BackedBy<TQuery> ({QueryType}) but also " +
                        "has a CustomFilterFunc. CustomFilterFunc is mutually exclusive with BackedBy — move " +
                        "the predicate onto the query record (so it travels to the server) and clear it here.",
                        typeof(TEntity).Name,
                        _entityDefinition?.BackedByQueryType?.Name ?? "<query>");
                }

                IsLoading = false;
                StateHasChanged();
                return;
            }

            // In server-paged mode the grid drives initial + subsequent loads via LoadPageAsync
            if (IsServerPaged)
            {
                if (CustomFilterFunc is not null || EntityFilters.Count > 0)
                {
                    _logger?.LogWarning(
                        "CnpDataSource<{Entity}> is configured for server paging but has client-side filters " +
                        "(CustomFilterFunc or EntityFilters). These are bypassed in server-paged mode because they " +
                        "would produce rows inconsistent with the server-returned TotalCount. " +
                        "Move filtering to the server side.",
                        typeof(TEntity).Name);
                }

                IsLoading = false;
                StateHasChanged();
                return;
            }

            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            StateHasChanged();

            var fetchAction = _entityDefinition?.CreateFetchAllAction(DependencyArgs?.Invoke());

            if (fetchAction is not null)
            {
                var fetchedEntities = await _dispatcher.DispatchAsync(fetchAction);

                _entityCache.Edit(innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(fetchedEntities);
                });
                
                ClearSelectedEntities();
            }

            IsLoading = false;
            StateHasChanged();
        }
        
        private void ClearSelectedEntities()
        {
            _selectedEntityCache.Clear();
            SelectedEntitiesCleared?.Invoke();
        }

        private void StateHasChanged()
        {
            OnStateHasChanged?.Invoke();
        }
        
        public void Refresh()
        {
            _entityCache.Refresh();
        }

        public async Task Reload()
        {
            await LoadData();
        }

        // Set when AttachQueryState is called with a paging mutator. Lets
        // LoadPageAsync (driven by paginator clicks and the search box in
        // CnpEntityDataGrid) fold those changes back into the typed query.
        private Action<int, int, string?>? _pagingApplier;

        public void AttachQueryState<TQuery>(
            QueryState<TQuery> queryState,
            Func<TQuery, int, int, string?, TQuery>? pagingMutator = null
        ) where TQuery : class
        {
            ArgumentNullException.ThrowIfNull(queryState);

            if (!IsBackedByQuery)
                throw new InvalidOperationException(
                    $"CnpDataSource<{typeof(TEntity).Name}> is not configured with BackedBy<TQuery>; " +
                    $"AttachQueryState is invalid here. Use BackedBy<{typeof(TQuery).Name}>(...) on the " +
                    "entity definition first.");

            // Last-write-wins: a new query supersedes any in-flight fetch.
            queryState.Changes
                .Select(query => Observable.FromAsync(() => LoadFromQuery(query)))
                .Switch()
                .Subscribe()
                .DisposeWith(_garbage);

            if (pagingMutator is not null)
            {
                _pagingApplier = (pageIndex, pageSize, search) =>
                    queryState.Mutate(current => pagingMutator(current, pageIndex, pageSize, search));
            }
        }

        private async Task LoadFromQuery(object query)
        {
            if (_entityDefinition is null)
                return;

            IsLoading = true;
            StateHasChanged();

            try
            {
                var fetchAction = _entityDefinition.CreateBackedByAction(query);
                var pagedResult = await _dispatcher.DispatchAsync(fetchAction);

                TotalCount = pagedResult.TotalCount;

                _entityCache.Edit(innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(pagedResult.Items);
                });

                ClearSelectedEntities();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "BackedBy fetch failed for CnpDataSource<{Entity}> (query: {QueryType}).",
                    typeof(TEntity).Name,
                    query?.GetType().Name ?? "<null>");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        public async Task LoadPageAsync(int pageIndex, int pageSize, string? search = null)
        {
            // BackedBy mode: fold paging + search into the typed query and let
            // the existing QueryState.Changes -> Switch -> LoadFromQuery pipeline
            // do the actual fetch. No-op if AttachQueryState wasn't given a
            // pagingMutator (paging/search just won't trigger fetches in that case).
            if (IsBackedByQuery)
            {
                _pagingApplier?.Invoke(pageIndex, pageSize, search);
                return;
            }

            if (_entityDefinition?.HasFetchPagedAction != true)
                return;

            IsLoading = true;
            StateHasChanged();

            var fetchAction = DependencyArgs?.Invoke() is { } args
                ? _entityDefinition.CreateFetchPagedAction(args, pageIndex, pageSize, search)
                : _entityDefinition.CreateFetchPagedAction(pageIndex, pageSize, search);

            var pagedResult = await _dispatcher.DispatchAsync(fetchAction);

            TotalCount = pagedResult.TotalCount;

            _entityCache.Edit(innerCache =>
            {
                innerCache.Clear();
                innerCache.AddOrUpdate(pagedResult.Items);
            });

            ClearSelectedEntities();

            IsLoading = false;
            StateHasChanged();
        }
        
        public void SelectItem(TEntity entity)
        {
            if (!MultiSelect)
                _selectedEntityCache.Clear();
            
            _selectedEntityCache.AddOrUpdate(entity);
        }

        public void DeselectItem(TEntity? entity)
        {
            if (entity is null)
                return;
            
            _selectedEntityCache.RemoveKey(entity.Id);
        }

        public bool IsSelected(TEntity entity)
        {
            return _selectedEntityCache.Lookup(entity.Id).HasValue;
        }

        public bool? IsAllSelected()
        {
            var isAllSelected = Entities?.All(IsSelected);
            return isAllSelected;
        }
        
        public async Task<TEntity?> Add()
        {
            var action = _entityDefinition?.CreateOpenAddDialogAction(DependencyArgs?.Invoke());
        
            if (action is null)
                return default;
            
            var createdEntity = await _dispatcher.DispatchAsync(action);

            return createdEntity;
        }

        public async Task Edit(TEntity entity)
        {
            var action = _entityDefinition?.CreateOpenEditDialogAction(entity);
        
            if (action is null)
                return;
        
            await _dispatcher.DispatchAsync(action);
        }

        public async Task Remove(IEnumerable<TEntity> entities)
        {
            // If the entity definition has a factory for creating a remove multiple dialog action, use it
            if (_entityDefinition?.HasOpenRemoveMultipleDialogActionFactory == true)
            {
                var action = _entityDefinition?.CreateOpenRemoveMultipleDialogAction(entities);
                
                if (action is null)
                    return;
                
                await _dispatcher.DispatchAsync(action);
                return;
            }

            // Otherwise, open a remove dialog for each entity
            foreach (var entity in entities)
            {
                var action = _entityDefinition?.CreateOpenRemoveDialogAction(entity);
                
                if (action is null)
                    return;
            
                await _dispatcher.DispatchAsync(action);
            }
        }

        public void Navigate(TEntity entity)
        {
            var action = _entityDefinition?.CreateNavigationAction(entity);

            if (action is null)
                return;
        
            _dispatcher.Dispatch(action);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        private IDisposable? _filtersSubscription;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _filtersSubscription?.Dispose();
                _actionSubscriber?.UnsubscribeFromAllActions(this);
            _garbage.Dispose();
                _entityCache.Dispose();
                _selectedEntityCache.Dispose();
                
                SelectedEntitiesChanged = null;
                SelectedEntitiesCleared = null;
                CustomFilterFunc = null;
                DependencyArgs = null;
                OnStateHasChanged = null;
            }
        }
    }
}

public enum EntityFilterType
{
    None,
    Default
}