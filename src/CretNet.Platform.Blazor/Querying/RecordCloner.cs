using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CretNet.Platform.Blazor.Querying;

/// <summary>
/// Runtime equivalent of the C# <c>with</c>-expression. Lets the generic
/// binding helpers mutate an immutable record's property without the
/// caller having to spell out a setter expression on every binding.
/// </summary>
/// <remarks>
/// <para>
/// Records compile <c>init</c>-only setters that reflection can still
/// invoke. The clone uses <c>Object.MemberwiseClone</c> (cheap shallow
/// copy) and the property is then assigned via a compiled
/// <see cref="Expression"/> setter cached per (type, property name) pair.
/// First call per pair compiles the setter; subsequent calls are
/// near-direct property assignment in cost.
/// </para>
/// <para>
/// Trade-off vs strategy (a) "explicit setter passed by the caller":
/// less ceremony at the call site, one tiny piece of reflection magic in
/// the helper. Acceptable because UI mutations are infrequent; if this
/// ever shows up in a profiler, swap in a typed Expression-compiled
/// setter without changing the public API.
/// </para>
/// </remarks>
internal static class RecordCloner
{
    private static readonly MethodInfo MemberwiseCloneMethod =
        typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly ConcurrentDictionary<(Type Type, string Property), Action<object, object?>> SetterCache = new();

    /// <summary>
    /// Returns a clone of <paramref name="source"/> with
    /// <paramref name="propertyName"/> set to <paramref name="value"/>.
    /// Throws <see cref="ArgumentException"/> if the property doesn't
    /// exist on <typeparamref name="T"/>.
    /// </summary>
    public static T With<T>(T source, string propertyName, object? value) where T : class
    {
        ArgumentNullException.ThrowIfNull(source);

        var clone = (T)MemberwiseCloneMethod.Invoke(source, null)!;
        var setter = SetterCache.GetOrAdd(
            (typeof(T), propertyName),
            key => CompileSetter(key.Type, key.Property));
        setter(clone, value);
        return clone;
    }

    private static Action<object, object?> CompileSetter(Type type, string propertyName)
    {
        var prop = type.GetProperty(propertyName)
            ?? throw new ArgumentException(
                $"Property '{propertyName}' not found on {type.FullName}.",
                nameof(propertyName));

        if (prop.SetMethod is null)
            throw new ArgumentException(
                $"Property '{propertyName}' on {type.FullName} has no settable accessor.",
                nameof(propertyName));

        // (object instance, object? value) => ((T)instance).Property = (TValue)value
        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");
        var assign = Expression.Assign(
            Expression.Property(Expression.Convert(instanceParam, type), prop),
            Expression.Convert(valueParam, prop.PropertyType));
        return Expression.Lambda<Action<object, object?>>(assign, instanceParam, valueParam).Compile();
    }

    /// <summary>
    /// Extracts the property name from a simple member-access expression
    /// like <c>q =&gt; q.IncludeStatuses</c>. Throws if the expression
    /// isn't a single property access (no method calls, no chains, no
    /// indexers).
    /// </summary>
    public static string PropertyNameFrom<T, V>(Expression<Func<T, V>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var body = expression.Body;
        if (body is UnaryExpression { Operand: MemberExpression unary })
            body = unary;

        if (body is MemberExpression { Member: PropertyInfo prop })
            return prop.Name;

        throw new ArgumentException(
            "Expression must be a simple property access (e.g. q => q.Field).",
            nameof(expression));
    }
}
