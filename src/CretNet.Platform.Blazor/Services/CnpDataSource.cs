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
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace CretNet.Platform.Blazor.Services
{
    public interface ICnpDataSource<TEntity, TId> : IDisposable
        where TEntity : IIdentity<TId>
        where TId : notnull
    {
        ReadOnlyObservableCollection<TEntity>? Entities { get; }
        ReadOnlyObservableCollection<TEntity>? SelectedEntities { get; }
        bool IsLoading { get; }
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
        void SelectItem(TEntity entity);
        void DeselectItem(TEntity? entity);
        bool IsSelected(TEntity entity);
        bool? IsAllSelected();
        ObservableCollection<EntityFilter<TEntity>> EntityFilters { get; }
    }

    public class CnpDataSource<TEntity, TId> : ICnpDataSource<TEntity, TId>
        where TEntity : IIdentity<TId>
        where TId : notnull
    {
        public bool IsLoading { get; private set; } = true;

        private readonly IActionSubscriber _actionSubscriber;
        private readonly IDispatcher _dispatcher;
        private readonly IEntityDefinition<TEntity, TId>? _entityDefinition;

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
        }

        public async Task Init()
        {
            EntityFilters.Clear();
            foreach (var filter in _entityDefinition?.GetEntityFilters()?.OfType<EntityFilter<TEntity>>() ?? Enumerable.Empty<EntityFilter<TEntity>>())
                EntityFilters.Add(filter);

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
