using System.Linq.Expressions;
using System.Reflection;
using Fluxor;
using CretNet.Platform.Blazor.Models;
using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;
using SortDirection = DynamicData.Binding.SortDirection;

namespace CretNet.Platform.Blazor.Services;

public interface IEntityDefinitionBuilder<TEntity, TId>
{
    IEntityDefinitionBuilder<TEntity, TId> WithLabel(string label);
    IEntityDefinitionBuilder<TEntity, TId> WithPluralLabel(string pluralLabel);
    IEntityDefinitionBuilder<TEntity, TId> WithIcon(Icon icon);
    IEntityDefinitionBuilder<TEntity, TId> WithIdentifier(Expression<Func<TEntity, string?>> identifierSelector);
    IEntityDefinitionBuilder<TEntity, TId> WithDisplayName(Expression<Func<TEntity, string?>> displayNameSelector);
    IEntityDefinitionBuilder<TEntity, TId> WithDataGrid<TCnpDataGrid>();
    IEntityDefinitionBuilder<TEntity, TId> WithFetchAllAction<TAction>(Func<TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>>;
    IEntityDefinitionBuilder<TEntity, TId> WithFetchAllAction<TParams, TAction>(Func<TParams, TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>>;
    IEntityDefinitionBuilder<TEntity, TId> WithFetchAction<TAction>(Func<TId, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithNavigationAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithNavigationAction<TAction>(Func<TId, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithOpenAddDialogAction<TAction>(Func<TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithOpenAddDialogAction<TParams, TAction>(Func<TParams, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithOpenEditDialogAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithOpenRemoveDialogAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>;
    IEntityDefinitionBuilder<TEntity, TId> WithOpenRemoveMultipleDialogAction<TAction>(Func<IEnumerable<TEntity>, TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>?>;
    IEntityDefinitionBuilder<TEntity, TId> WithCreateAction<TAction>() where TAction : ICnpEntityAction<TEntity>;
    IEntityDefinitionBuilder<TEntity, TId> WithUpdateAction<TAction>() where TAction : ICnpEntityAction<TEntity>;
    IEntityDefinitionBuilder<TEntity, TId> WithDeleteAction<TAction>() where TAction : ICnpEntityAction<TEntity>;
    IEntityDefinitionBuilder<TEntity, TId> WithRefreshEvent<TEvent>() where TEvent : ICnpEvent;
    IEntityDefinitionBuilder<TEntity, TId> WithReloadEvent<TEvent>() where TEvent : ICnpEvent;
    IEntityDefinitionBuilder<TEntity, TId> WithFilter(Func<string, TEntity, bool> filterFunc);
    IEntityDefinitionBuilder<TEntity, TId> WithSort(Func<TEntity, IComparable> sortByFunc, SortDirection sortOrder = SortDirection.Ascending);
    IEntityDefinitionBuilder<TEntity, TId> WithEntityFilter(string category, int sequence, string key, bool enabled, Func<IQueryable<TEntity>, IQueryable<TEntity>> filter);
}

public interface IEntityDefinition
{
    string EntityType { get; }
    string Label { get; }
    string PluralLabel { get; }
    Icon? Icon { get; }
    object CreateIdNavigationAction(string id);
    string? GetIdentifier(object entity);
}

public interface IEntityDefinition<TEntity, TId> : IEntityDefinition
{
    Func<string, TEntity, bool> FilterFunc { get; }
    Func<TEntity, IComparable> SortByFunc { get; }
    public SortDirection SortOrder { get; }
    bool HasFetchAllAction { get; }
    bool HasFetchAction { get; }
    bool HasNavigationAction { get; }
    bool HasOpenAddDialogAction { get; }
    bool HasCreateAction { get; }
    bool HasOpenEditDialogAction { get; }
    bool HasUpdateAction { get; }
    bool HasOpenRemoveDialogAction { get; }
    bool HasOpenRemoveMultipleDialogActionFactory { get; }
    Type DataGridType { get; }
    bool HasDataGrid { get; }
    string? GetIdentifier(TEntity entity);
    string? GetDisplayName(TEntity entity);
    ICnpEntityAction<IEnumerable<TEntity>> CreateFetchAllAction();
    ICnpEntityAction<IEnumerable<TEntity>> CreateFetchAllAction<TParams>(TParams parameters);
    ICnpEntityAction<TEntity?> CreateFetchAction(TId id);
    ICnpEntityAction<TEntity?> CreateNavigationAction(TEntity entity);
    ICnpEntityAction<TEntity?> CreateIdNavigationAction(TId id);
    ICnpEntityAction<TEntity?> CreateOpenAddDialogAction();
    ICnpEntityAction<TEntity?> CreateOpenAddDialogAction<TParams>(TParams parameters);
    ICnpEntityAction<TEntity?> CreateCreateAction(TEntity entity);
    ICnpEntityAction<TEntity?> CreateOpenEditDialogAction(TEntity entity);
    ICnpEntityAction<TEntity?> CreateUpdateAction(TEntity entity);
    ICnpEntityAction<TEntity?> CreateOpenRemoveDialogAction(TEntity entity);
    ICnpEntityAction<IEnumerable<TEntity>?> CreateOpenRemoveMultipleDialogAction(IEnumerable<TEntity> entities);
    void SubscribeCreateSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback);
    void SubscribeUpdateSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback);
    void SubscribeDeleteSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback);
    void SubscribeRefresh(IActionSubscriber actionSubscriber, object subscriber, Action callback);
    void SubscribeReload(IActionSubscriber actionSubscriber, object subscriber, Action callback);
    IEnumerable<IEntityFilter> GetEntityFilters();
}

public abstract class EntityDefinition<TEntity, TId> : IEntityDefinition<TEntity, TId>, IEntityDefinitionBuilder<TEntity, TId>
    where TEntity : IIdentity<TId>
{
    // Define
    private Expression<Func<TEntity, string?>>? _identifierSelector;
    private Func<TEntity, string?>? _compiledIdentifierSelector;
    private Expression<Func<TEntity, string?>>? _displayNameSelector;
    private Func<TEntity, string?>? _compiledDisplayNameSelector;
    private Type? _dataGridType;

    private Func<ICnpEntityAction<IEnumerable<TEntity>>>? _fetchAllActionFactory;
    private Func<object, ICnpEntityAction<IEnumerable<TEntity>>>? _fetchAllWithParamsActionFactory;
    private Func<TId, ICnpEntityAction<TEntity?>>? _fetchActionFactory;
    private Func<object, ICnpEntityAction<TEntity?>>? _navigationActionFactory;
    private bool? _navigationActionIsById;
    private Func<ICnpEntityAction<TEntity?>>? _openAddDialogActionFactory;
    private Func<object, ICnpEntityAction<TEntity?>>? _openAddDialogWithParamsActionFactory;
    private Func<TEntity, ICnpEntityAction<TEntity?>>? _createActionFactory;
    private Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>? _subscribeCreateSuccessFactory;
    private Func<TEntity, ICnpEntityAction<TEntity?>>? _openEditDialogActionFactory;
    private Func<TEntity, ICnpEntityAction<TEntity?>>? _updateActionFactory;
    private Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>? _subscribeUpdateSuccessFactory;
    private Func<TEntity, ICnpEntityAction<TEntity?>>? _openRemoveDialogActionFactory;
    private Func<IEnumerable<TEntity>, ICnpEntityAction<IEnumerable<TEntity>?>>? _openRemoveMultipleDialogActionFactory;
    private Func<TEntity, ICnpEntityAction<TEntity?>>? _deleteActionFactory;
    private Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>? _subscribeDeleteSuccessFactory;
    private readonly List<Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>> _createActionSuccessSubscriptionFactories = [];
    private readonly List<Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>> _updateActionSuccessSubscriptionFactories = [];
    private readonly List<Action<IActionSubscriber, object, Action<ICnpEntitySuccessAction<TEntity>>>> _deleteActionSuccessSubscriptionFactories = [];
    private readonly List<Action<IActionSubscriber, object, Action>> _refreshEventSubscriptionFactories = [];
    private readonly List<Action<IActionSubscriber, object, Action>> _reloadEventSubscriptionFactories = [];
    private readonly List<Func<IEntityFilter>> _filterFactories = [];

    public IEntityDefinitionBuilder<TEntity, TId> Entity()
    {
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithLabel(string label)
    {
        Label = label;
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithPluralLabel(string pluralLabel)
    {
        PluralLabel = pluralLabel;
        return this;
    }
    
    public IEntityDefinitionBuilder<TEntity, TId> WithIcon(Icon icon)
    {
        Icon = icon;
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithIdentifier(Expression<Func<TEntity, string?>> identifierSelector)
    {
        _identifierSelector = identifierSelector;
        _compiledIdentifierSelector = identifierSelector.Compile();
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithDisplayName(Expression<Func<TEntity, string?>> displayNameSelector)
    {
        _displayNameSelector = displayNameSelector;
        _compiledDisplayNameSelector = displayNameSelector.Compile();
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithDataGrid<TCnpDataGrid>()
    {
        _dataGridType = typeof(TCnpDataGrid);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithFetchAllAction<TAction>(Func<TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>>
    {
        _fetchAllActionFactory = () => factory();
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithFetchAllAction<TParams, TAction>(Func<TParams, TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>>
    {
        _fetchAllWithParamsActionFactory = (parameters) => factory((TParams)parameters);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithFetchAction<TAction>(Func<TId, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _fetchActionFactory = (id) => factory(id);
        return this;
    }
    
    public IEntityDefinitionBuilder<TEntity, TId> WithNavigationAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _navigationActionIsById = false;
        _navigationActionFactory = input =>
        {
            if (input is not TEntity entity)
                throw new ArgumentException("Entity must be of type TEntity.", nameof(entity));

            return factory(entity);
        };
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithNavigationAction<TAction>(Func<TId, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _navigationActionIsById = true;
        _navigationActionFactory = input =>
        {
            if (input is not TId id)
                throw new ArgumentException("Id must be of type TId.", nameof(id));

            return factory(id);
        };
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithOpenAddDialogAction<TAction>(Func<TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _openAddDialogActionFactory = () => factory();
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithOpenAddDialogAction<TParams, TAction>(Func<TParams, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _openAddDialogWithParamsActionFactory = (parameters) => factory((TParams)parameters);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithCreateAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _createActionFactory = (entity) => factory(entity);

        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _subscribeCreateSuccessFactory = (actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]);

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithOpenEditDialogAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _openEditDialogActionFactory = (entity) => factory(entity);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithUpdateAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _updateActionFactory = (entity) => factory(entity);

        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _subscribeUpdateSuccessFactory = (actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]);

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithOpenRemoveDialogAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _openRemoveDialogActionFactory = (entity) => factory(entity);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithOpenRemoveMultipleDialogAction<TAction>(Func<IEnumerable<TEntity>, TAction> factory) where TAction : ICnpEntityAction<IEnumerable<TEntity>?>
    {
        _openRemoveMultipleDialogActionFactory = (entities) => factory(entities);
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithDeleteAction<TAction>(Func<TEntity, TAction> factory) where TAction : ICnpEntityAction<TEntity?>
    {
        _deleteActionFactory = (entity) => factory(entity);

        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _subscribeDeleteSuccessFactory = (actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]);

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithCreateAction<TAction>() where TAction : ICnpEntityAction<TEntity>
    {
        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _createActionSuccessSubscriptionFactories.Add((actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]));

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithUpdateAction<TAction>() where TAction : ICnpEntityAction<TEntity>
    {
        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _updateActionSuccessSubscriptionFactories.Add((actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]));

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithDeleteAction<TAction>() where TAction : ICnpEntityAction<TEntity>
    {
        var methodInfo = typeof(TAction).GetMethod("SubscribeSuccess", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
            return this;

        _deleteActionSuccessSubscriptionFactories.Add((actionSubscriber, subscriber, callback) => methodInfo.Invoke(null, [actionSubscriber, subscriber, callback]));

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithRefreshEvent<TEvent>() where TEvent : ICnpEvent
    {
        _refreshEventSubscriptionFactories.Add((actionSubscriber, subscriber, callback) => actionSubscriber.SubscribeToAction<TEvent>(subscriber, _ => callback()));

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithReloadEvent<TEvent>() where TEvent : ICnpEvent
    {
        _reloadEventSubscriptionFactories.Add((actionSubscriber, subscriber, callback) => actionSubscriber.SubscribeToAction<TEvent>(subscriber, _ => callback()));

        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithFilter(Func<string, TEntity, bool> filterFunc)
    {
        FilterFunc = filterFunc;
        return this;
    }

    public IEntityDefinitionBuilder<TEntity, TId> WithSort(Func<TEntity, IComparable> sortByFunc, SortDirection sortOrder = SortDirection.Ascending)
    {
        SortByFunc = sortByFunc;
        SortOrder = sortOrder;
        return this;
    }
    public IEntityDefinitionBuilder<TEntity, TId> WithEntityFilter(string category, int sequence, string key, bool enabled, Func<IQueryable<TEntity>, IQueryable<TEntity>> filter)
    {
        _filterFactories.Add(() => new EntityFilter<TEntity>
        {
            Category = category,
            Sequence = sequence,
            Key = key,
            Enabled = enabled,
            DefaultEnabled = enabled,
            Filter = filter
        });

        return this;
    }

    // Use
    public string EntityType => typeof(TEntity).Name ?? throw new InvalidOperationException("Entity type name is not defined.");
    public string Label { get; private set; } = string.Empty;
    public string PluralLabel { get; private set; } = string.Empty;
    public Icon? Icon { get; private set; }
    public Func<string, TEntity, bool> FilterFunc { get; private set; } = (_, __) => true;
    public Func<TEntity, IComparable> SortByFunc { get; private set; } = _ => string.Empty;
    public SortDirection SortOrder { get; private set; } = SortDirection.Ascending;

    public string? GetIdentifier(TEntity entity)
    {
        return _compiledIdentifierSelector?.Invoke(entity);
    }
    
    public string? GetIdentifier(object entity)
    {
        if (entity is not TEntity typedEntity)
            throw new ArgumentException($"Entity must be of type {typeof(TEntity).Name}.", nameof(entity));
        
        return _compiledIdentifierSelector?.Invoke(typedEntity);
    }

    public string? GetDisplayName(TEntity entity)
    {
        return _compiledDisplayNameSelector?.Invoke(entity);
    }

    public bool HasDataGrid => _dataGridType != null;
    public Type DataGridType => _dataGridType ?? throw new InvalidOperationException("DataGridType has not been defined.");

    public bool HasFetchAllAction => _fetchAllActionFactory != null || _fetchAllWithParamsActionFactory != null;
    public ICnpEntityAction<IEnumerable<TEntity>> CreateFetchAllAction()
    {
        if (_fetchAllActionFactory == null)
            throw new InvalidOperationException("FetchAllAction has not been defined.");

        return _fetchAllActionFactory();
    }

    public ICnpEntityAction<IEnumerable<TEntity>> CreateFetchAllAction<TParams>(TParams parameters)
    {
        if (parameters is null)
            return CreateFetchAllAction();

        if (_fetchAllWithParamsActionFactory == null)
            throw new InvalidOperationException("FetchAllAction has not been defined.");

        return _fetchAllWithParamsActionFactory(parameters);
    }

    public bool HasFetchAction => _fetchActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateFetchAction(TId id)
    {
        if (_fetchActionFactory == null)
            throw new InvalidOperationException("FetchAction has not been defined.");

        return _fetchActionFactory(id);
    }

    public bool HasNavigationAction => _navigationActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateNavigationAction(TEntity entity)
    {
        if (_navigationActionFactory is null || !_navigationActionIsById.HasValue)
            throw new InvalidOperationException("NavigationAction has not been defined.");

        return _navigationActionIsById.Value ? CreateIdNavigationAction(entity.Id) : _navigationActionFactory(entity);
    }

    public ICnpEntityAction<TEntity?> CreateIdNavigationAction(TId id)
    {
        if (_navigationActionFactory is null || !_navigationActionIsById.HasValue)
            throw new InvalidOperationException("NavigationAction has not been defined.");

        if (!_navigationActionIsById.Value)
            throw new InvalidOperationException("NavigationAction is not defined by Id.");

        return _navigationActionFactory(id);
    }
    
    public object CreateIdNavigationAction(string id)
    {
        object typedId;
    
        if (typeof(TId) == typeof(Guid))
        {
            typedId = Guid.Parse(id);
        }
        else if (typeof(TId) == typeof(int))
        {
            typedId = int.Parse(id);
        }
        else if (typeof(TId) == typeof(string))
        {
            typedId = id;
        }
        else
        {
            throw new NotSupportedException($"Type {typeof(TId).Name} is not supported for id parsing.");
        }
    
        return CreateIdNavigationAction((TId)typedId);
    }

    public bool HasOpenAddDialogAction => _openAddDialogActionFactory != null || _openAddDialogWithParamsActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateOpenAddDialogAction()
    {
        if (_openAddDialogActionFactory == null)
            throw new InvalidOperationException("OpenAddDialogAction has not been defined.");

        return _openAddDialogActionFactory();
    }

    public ICnpEntityAction<TEntity?> CreateOpenAddDialogAction<TParams>(TParams parameters)
    {
        if (parameters is null)
            return CreateOpenAddDialogAction();

        if (_openAddDialogWithParamsActionFactory == null)
            throw new InvalidOperationException("OpenAddDialogAction has not been defined.");

        return _openAddDialogWithParamsActionFactory(parameters);
    }

    public bool HasCreateAction => _createActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateCreateAction(TEntity entity)
    {
        if (_createActionFactory == null)
            throw new InvalidOperationException("CreateAction has not been defined.");

        return _createActionFactory(entity);
    }

    public void SubscribeCreateSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback)
    {
        _subscribeCreateSuccessFactory?.Invoke(actionSubscriber, subscriber, callback);
        _createActionSuccessSubscriptionFactories.ForEach(factory => factory.Invoke(actionSubscriber, subscriber, callback));
    }

    public bool HasOpenEditDialogAction => _openEditDialogActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateOpenEditDialogAction(TEntity entity)
    {
        if (_openEditDialogActionFactory == null)
            throw new InvalidOperationException("OpenEditDialogAction has not been defined.");

        return _openEditDialogActionFactory(entity);
    }

    public bool HasUpdateAction => _updateActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateUpdateAction(TEntity entity)
    {
        if (_updateActionFactory == null)
            throw new InvalidOperationException("UpdateAction has not been defined.");

        return _updateActionFactory(entity);
    }

    public void SubscribeUpdateSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback)
    {
        _subscribeUpdateSuccessFactory?.Invoke(actionSubscriber, subscriber, callback);
        _updateActionSuccessSubscriptionFactories.ForEach(factory => factory.Invoke(actionSubscriber, subscriber, callback));
    }

    public bool HasOpenRemoveDialogAction => _openRemoveDialogActionFactory != null;
    public ICnpEntityAction<TEntity?> CreateOpenRemoveDialogAction(TEntity entity)
    {
        if (_openRemoveDialogActionFactory == null)
            throw new InvalidOperationException("OpenRemoveDialogAction has not been defined.");

        return _openRemoveDialogActionFactory(entity);
    }

    public bool HasOpenRemoveMultipleDialogActionFactory => _openRemoveMultipleDialogActionFactory != null;
    public ICnpEntityAction<IEnumerable<TEntity>?> CreateOpenRemoveMultipleDialogAction(IEnumerable<TEntity> entities)
    {
        if (_openRemoveMultipleDialogActionFactory == null)
            throw new InvalidOperationException("OpenRemoveMultipleDialogAction has not been defined.");

        return _openRemoveMultipleDialogActionFactory(entities);
    }


    public void SubscribeDeleteSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<TEntity>> callback)
    {
        _subscribeDeleteSuccessFactory?.Invoke(actionSubscriber, subscriber, callback);
        _deleteActionSuccessSubscriptionFactories.ForEach(factory => factory.Invoke(actionSubscriber, subscriber, callback));
    }

    public void SubscribeRefresh(IActionSubscriber actionSubscriber, object subscriber, Action callback)
    {
        _refreshEventSubscriptionFactories.ForEach(factory => factory.Invoke(actionSubscriber, subscriber, callback));
    }

    public void SubscribeReload(IActionSubscriber actionSubscriber, object subscriber, Action callback)
    {
        _reloadEventSubscriptionFactories.ForEach(factory => factory.Invoke(actionSubscriber, subscriber, callback));
    }

    public IEnumerable<IEntityFilter> GetEntityFilters()
    {
        return _filterFactories.Select(factory => factory());
    }
}