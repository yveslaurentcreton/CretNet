using Bogus;
using DotNetJet.Utilities;
using FluentAssertions;
using Xunit;

namespace DotNetJet.Tests.Utilities;

public class JetLinqUtilitiesTests
{
    private readonly Faker _faker;

    public JetLinqUtilitiesTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public async Task SelectManyAsync_ShouldFlattenResults_WhenGivenValidInput()
    {
        // Arrange
        var input = Enumerable.Range(1, 10).Select(_ => _faker.Random.Int()).ToList();
        var expectedResult = input.SelectMany(x => new[] { x, x + 1 }).ToList();
        Func<int, Task<IEnumerable<int>>> func = x => Task.FromResult<IEnumerable<int>>(new[] { x, x + 1 });

        // Act
        var result = await JetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void SelectManyAsync_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        // Arrange
        IEnumerable<int> input = null;
        Func<int, Task<IEnumerable<int>>> func = x => Task.FromResult<IEnumerable<int>>(new[] { x });

        // Act
        Func<Task> act = async () => await JetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void SelectManyAsync_ShouldThrowArgumentNullException_WhenFuncIsNull()
    {
        // Arrange
        var input = Enumerable.Range(1, 10).Select(_ => _faker.Random.Int()).ToList();
        Func<int, Task<IEnumerable<int>>> func = null;

        // Act
        Func<Task> act = async () => await JetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SelectManyAsync_ShouldHandleEmptyCollection_WithoutErrors()
    {
        // Arrange
        var input = Enumerable.Empty<int>();
        Func<int, Task<IEnumerable<int>>> func = x => Task.FromResult<IEnumerable<int>>(new[] { x });

        // Act
        var result = await JetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SelectManyAsync_ShouldPropagateException_WhenFuncThrowsException()
    {
        // Arrange
        var input = Enumerable.Range(1, 10).Select(_ => _faker.Random.Int()).ToList();
        Func<int, Task<IEnumerable<int>>> func = x => throw new InvalidOperationException();

        // Act
        Func<Task> act = async () => await JetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
