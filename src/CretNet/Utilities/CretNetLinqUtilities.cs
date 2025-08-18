namespace CretNet.Utilities;

/// <summary>
/// Provides extension and utility methods for LINQ to enhance and simplify common operations, including asynchronous operations on collections.
/// </summary>
public static class CretNetLinqUtilities
{
    /// <summary>
    /// Projects each element of a sequence to an IEnumerable<T1> and flattens the resulting sequences into one sequence asynchronously.
    /// </summary>
    /// <param name="enumeration">The sequence of elements to project and flatten.</param>
    /// <param name="func">A transform function to apply to each element, which returns an asynchronous sequence.</param>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="T1">The type of the elements of the resulting sequence.</typeparam>
    /// <returns>A Task representing the asynchronous operation, containing a flattened sequence of the projected elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumeration"/> or <paramref name="func"/> is null.</exception>
    public static async Task<IEnumerable<T1>> SelectManyAsync<T, T1>(
        this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<T1>>> func)
    {
        ArgumentNullException.ThrowIfNull(enumeration);
        ArgumentNullException.ThrowIfNull(func);

        return (await Task.WhenAll(enumeration.Select(func))).SelectMany(x => x);
    }
}
