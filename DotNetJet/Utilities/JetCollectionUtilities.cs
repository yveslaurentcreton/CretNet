namespace DotNetJet.Utilities;

/// <summary>
/// Provides extension and utility methods for collections to enhance and simplify common operations, such as adding or removing multiple items at once.
/// </summary>
public static class JetCollectionUtilities
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the ICollection.
    /// </summary>
    /// <param name="collection">The collection to add items to.</param>
    /// <param name="items">The items to add to the collection.</param>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> or <paramref name="items"/> is null.</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Removes the elements in the specified collection from the ICollection.
    /// </summary>
    /// <param name="collection">The collection to remove items from.</param>
    /// <param name="items">The items to remove from the collection.</param>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> or <paramref name="items"/> is null.</exception>
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(items);

        // Optimization for List<T> to use RemoveAll for efficiency
        if (collection is List<T> list)
        {
            list.RemoveAll(items.Contains);
        }
        else
        {
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }
    }
}
