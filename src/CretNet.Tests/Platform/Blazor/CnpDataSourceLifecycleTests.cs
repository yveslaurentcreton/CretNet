using System.Diagnostics.CodeAnalysis;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using CretNet.Platform.Querying;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Platform.Blazor;

/// <summary>
/// Regression tests for the loading-state machine on
/// <see cref="CnpDataSource{TEntity, TId}"/>. The original bug:
/// in BackedBy mode, <see cref="ICnpDataSource{TEntity, TId}.IsLoading"/>
/// flipped to <c>false</c> when <see cref="CnpDataSource{TEntity, TId}.Init"/>
/// returned, but the first fetch was only triggered later by the page's
/// <c>AfterInit</c> hook calling <c>AttachQueryState</c>. Between Init's
/// return and the first fetch arriving, the grid rendered an empty body
/// with the loading overlay gone — flash of "no data" before the data.
///
/// The fix: keep <c>IsLoading=true</c> across Init in BackedBy mode and
/// expose <see cref="ICnpDataSource{TEntity, TId}.HasLoadedOnce"/> for
/// the rendering layer to gate the FluentDataGrid on.
/// </summary>
public class CnpDataSourceLifecycleTests
{
    [Fact]
    public void NewInstance_StartsInLoadingState_WithHasLoadedOnceFalse()
    {
        var sut = CreateBackedByDataSource();

        sut.IsLoading.ShouldBeTrue("a freshly-constructed data source should be loading");
        sut.HasLoadedOnce.ShouldBeFalse("HasLoadedOnce flips only after the first fetch resolves");
        sut.Entities.ShouldBeNull("the rx pipeline binds the collection during Init, not the constructor");
    }

    [Fact]
    public async Task Init_BackedBy_KeepsLoadingState_UntilFirstFetchResolves()
    {
        // Regression test for the loading flash: in BackedBy mode the page
        // (not Init) drives the first fetch via AttachQueryState. Init must
        // NOT flip IsLoading=false or HasLoadedOnce=true on return — that
        // window was the source of the "loading → no data → data" flash.
        var sut = CreateBackedByDataSource();

        await sut.Init();

        sut.IsLoading.ShouldBeTrue("Init must keep IsLoading=true in BackedBy mode (page drives first fetch via AttachQueryState)");
        sut.HasLoadedOnce.ShouldBeFalse("Init must not mark HasLoadedOnce until the first fetch completes");
    }

    private static CnpDataSource<TestRow, Guid> CreateBackedByDataSource()
    {
        // Real EntityDefinition (built via the public DSL) with BackedBy<>
        // wired up — gives the data source the IsBackedByQuery=true path.
        var entityDefinition = new TestEntityDefinition();

        var services = new ServiceCollection();
        services.AddSingleton<IEntityDefinition<TestRow, Guid>>(entityDefinition);

        var serviceProvider = services.BuildServiceProvider();
        var actionSubscriber = new StubActionSubscriber();
        var dispatcher = new StubDispatcher();

        return new CnpDataSource<TestRow, Guid>(serviceProvider, actionSubscriber, dispatcher);
    }

    // ---------------- Test types ----------------

    private sealed record TestRow(Guid Id) : IIdentity<Guid>
    {
        Guid IIdentity<Guid>.Id => Id;
    }

    private sealed record TestQuery : IPagedQuery<TestRow>
    {
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? Search { get; init; }
        public SortSpec? Sort { get; init; }
    }

    private sealed class TestFetchAction : ICnpEntityAction<PagedResult<TestRow>>
    {
        public TestFetchAction(TestQuery _) { }
        public TaskCompletionSource<PagedResult<TestRow>> TaskCompletionSource { get; } = new();
        public bool SaveToState { get; set; }
        public Task<PagedResult<TestRow>> Effect(IDispatcher dispatcher) =>
            Task.FromResult(new PagedResult<TestRow> { Items = [], TotalCount = 0, PageIndex = 1, PageSize = 20 });
    }

    private sealed class TestEntityDefinition : EntityDefinition<TestRow, Guid>
    {
        public TestEntityDefinition()
        {
            Entity()
                .WithLabel("Test")
                .WithPluralLabel("Tests")
                .WithIdentifier(x => x.Id.ToString())
                .WithDisplayName(x => x.Id.ToString())
                .BackedBy<TestQuery, TestFetchAction>(query => new TestFetchAction(query));
        }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private sealed class StubActionSubscriber : IActionSubscriber
    {
        public void SubscribeToAction<TAction>(object subscriber, Action<TAction> callback) { }
        public void UnsubscribeFromAllActions(object subscriber) { }
        public IDisposable GetActionUnsubscriberAsIDisposable(object subscriber) =>
            new EmptyDisposable();

        private sealed class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }

    private sealed class StubDispatcher : IDispatcher
    {
        public void Dispatch(object action) { }
        public event EventHandler<ActionDispatchedEventArgs>? ActionDispatched;
        public bool IsDispatching => false;
    }
}
