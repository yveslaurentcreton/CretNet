using Bogus;
using DotNetJet.Utilities;
using FluentAssertions;
using System.Collections;

namespace DotNetJet.Tests.Utilities;

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
        collection.Should().BeEquivalentTo(itemsToAdd);
    }

    [Fact]
    public void AddRange_ShouldThrowArgumentNullException_WhenCollectionIsNull()
    {
        // Arrange
        TestCollection<int> collection = null;
        var itemsToAdd = new TestCollection<int> { _faker.Random.Int(), _faker.Random.Int() };

        // Act
        var act = new Action(() => collection.AddRange(itemsToAdd));
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRange_ShouldThrowArgumentNullException_WhenItemsIsNull()
    {
        // Arrange
        var collection = new TestCollection<int>();
        TestCollection<int> itemsToAdd = null;

        // Act
        var act = new Action(() => collection.AddRange(itemsToAdd));
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
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
        collection.Should().NotContain(itemsToRemove);
        collection.Should().Contain(item2);
    }

    [Fact]
    public void RemoveRange_ShouldThrowArgumentNullException_WhenCollectionIsNull()
    {
        // Arrange
        TestCollection<int> collection = null;
        var itemsToRemove = new TestCollection<int> { _faker.Random.Int() };

        // Act
        var act = new Action(() => collection.RemoveRange(itemsToRemove));
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveRange_ShouldThrowArgumentNullException_WhenItemsIsNull()
    {
        // Arrange
        var collection = new TestCollection<int> { _faker.Random.Int() };
        TestCollection<int> itemsToRemove = null;

        // Act
        var act = new Action(() => collection.RemoveRange(itemsToRemove));
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
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
        collection.Should().NotContain(itemsToRemove);
        collection.Should().Contain(item2);
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
