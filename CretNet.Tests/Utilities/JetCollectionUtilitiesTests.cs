using Bogus;
using CretNet.Utilities;
using Shouldly;
using System.Collections;

namespace CretNet.Tests.Utilities;

public class JetCollectionUtilitiesTests
{
    private readonly Faker _faker;

    public JetCollectionUtilitiesTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void AddRange_ShouldAddItems_WhenCollectionIsNotNull()
    {
        // Arrange
        var collection = new TestCollection<int>();
        var itemsToAdd = new TestCollection<int> { _faker.Random.Int(), _faker.Random.Int() };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        collection.ShouldBe(itemsToAdd);
    }

    [Fact]
    public void AddRange_ShouldThrowArgumentNullException_WhenCollectionIsNull()
    {
        // Arrange
        TestCollection<int> collection = null!;
        var itemsToAdd = new TestCollection<int> { _faker.Random.Int(), _faker.Random.Int() };

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => collection.AddRange(itemsToAdd));
    }

    [Fact]
    public void AddRange_ShouldThrowArgumentNullException_WhenItemsIsNull()
    {
        // Arrange
        var collection = new TestCollection<int>();
        TestCollection<int> itemsToAdd = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => collection.AddRange(itemsToAdd));
    }

    [Fact]
    public void RemoveRange_ShouldRemoveItems_WhenCollectionIsNotNull()
    {
        // Arrange
        var item1 = _faker.Random.Int();
        var item2 = _faker.Random.Int();
        var collection = new TestCollection<int> { item1, item2 };
        var itemsToRemove = new TestCollection<int> { item1 };

        // Act
        collection.RemoveRange(itemsToRemove);

        // Assert
        foreach (var item in itemsToRemove)
        {
            collection.ShouldNotContain(item);
        }
        collection.ShouldContain(item2);
    }

    [Fact]
    public void RemoveRange_ShouldThrowArgumentNullException_WhenCollectionIsNull()
    {
        // Arrange
        TestCollection<int> collection = null!;
        var itemsToRemove = new TestCollection<int> { _faker.Random.Int() };

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => collection.RemoveRange(itemsToRemove));
    }

    [Fact]
    public void RemoveRange_ShouldThrowArgumentNullException_WhenItemsIsNull()
    {
        // Arrange
        var collection = new TestCollection<int> { _faker.Random.Int() };
        TestCollection<int> itemsToRemove = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => collection.RemoveRange(itemsToRemove));
    }

    [Fact]
    public void RemoveRange_ShouldUseRemoveAll_WhenCollectionIsList()
    {
        // Arrange
        var item1 = _faker.Random.Int();
        var item2 = _faker.Random.Int();
        var collection = new List<int> { item1, item2 };
        var itemsToRemove = new TestCollection<int> { item1 };

        // Act
        collection.RemoveRange(itemsToRemove);

        // Assert
        foreach (var item in itemsToRemove)
        {
            collection.ShouldNotContain(item);
        }
        collection.ShouldContain(item2);
    }
}

public class TestCollection<T> : ICollection<T>
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;
    
    public bool IsReadOnly => false;

    public void Add(T item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public bool Contains(T item) => _items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    public bool Remove(T item) => _items.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
