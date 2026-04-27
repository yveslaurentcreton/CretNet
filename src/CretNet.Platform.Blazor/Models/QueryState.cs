using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace CretNet.Platform.Blazor.Models;

/// <summary>
/// Per-screen mutable wrapper around an immutable <typeparamref name="TQuery"/>
/// (typically a <c>record</c> implementing <see cref="CretNet.Platform.Querying.IPagedQuery{TRow}"/>).
/// The Blazor data source subscribes to <see cref="Changes"/> and refetches on
/// every emission.
/// </summary>
/// <remarks>
/// <para>
/// UI binding helpers (e.g. <c>BindCheckboxGroup(state, q =&gt; q.IncludeStatuses, ...)</c>)
/// mutate the wrapped query through <see cref="Mutate"/> using a
/// <c>with</c>-expression. The wrapped query stays immutable; only this
/// wrapper notifies.
/// </para>
/// <para>
/// The current value is also exposed synchronously via <see cref="Current"/>
/// so non-reactive consumers (server-side rendering, initial fetch) can read
/// it without subscribing.
/// </para>
/// </remarks>
public sealed class QueryState<TQuery> : IDisposable
    where TQuery : class
{
    private readonly BehaviorSubject<TQuery> _subject;
    private bool _disposed;

    public QueryState(TQuery initial)
    {
        _subject = new BehaviorSubject<TQuery>(initial);
    }

    /// <summary>
    /// Current query value. Always reflects the latest <see cref="Mutate"/>
    /// or <see cref="Set"/> call.
    /// </summary>
    public TQuery Current => _subject.Value;

    /// <summary>
    /// Cold observable of query values, starting with the current value.
    /// Subscribe in the data source to refetch on every change.
    /// </summary>
    public IObservable<TQuery> Changes => _subject.AsObservable();

    /// <summary>
    /// Replace the current query with <paramref name="query"/>. Triggers
    /// an emission on <see cref="Changes"/>.
    /// </summary>
    public void Set(TQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        _subject.OnNext(query);
    }

    /// <summary>
    /// Apply <paramref name="mutator"/> to the current query and publish
    /// the result. Use with a record's <c>with</c>-expression for clean,
    /// non-mutating updates:
    /// <code>state.Mutate(q =&gt; q with { IncludeStatuses = [TaskStatus.ToDo] });</code>
    /// </summary>
    public void Mutate(Func<TQuery, TQuery> mutator)
    {
        ArgumentNullException.ThrowIfNull(mutator);
        var next = mutator(_subject.Value);
        ArgumentNullException.ThrowIfNull(next);
        _subject.OnNext(next);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _subject.OnCompleted();
        _subject.Dispose();
    }
}
