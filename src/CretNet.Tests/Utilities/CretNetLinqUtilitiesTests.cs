using Bogus;
using CretNet.Utilities;
using Shouldly;

namespace CretNet.Tests.Utilities;

public class CretNetLinqUtilitiesTests
{
    private readonly Faker _faker;

    public CretNetLinqUtilitiesTests()
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
        var result = await CretNetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task SelectManyAsync_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        // Arrange
        IEnumerable<int> input = null!;
        Func<int, Task<IEnumerable<int>>> func = x => Task.FromResult<IEnumerable<int>>(new[] { x });

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => 
            await CretNetLinqUtilities.SelectManyAsync(input, func));
    }

    [Fact]
    public async Task SelectManyAsync_ShouldThrowArgumentNullException_WhenFuncIsNull()
    {
        // Arrange
        var input = Enumerable.Range(1, 10).Select(_ => _faker.Random.Int()).ToList();
        Func<int, Task<IEnumerable<int>>> func = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => 
            await CretNetLinqUtilities.SelectManyAsync(input, func));
    }

    [Fact]
    public async Task SelectManyAsync_ShouldHandleEmptyCollection_WithoutErrors()
    {
        // Arrange
        var input = Enumerable.Empty<int>();
        Func<int, Task<IEnumerable<int>>> func = x => Task.FromResult<IEnumerable<int>>(new[] { x });

        // Act
        var result = await CretNetLinqUtilities.SelectManyAsync(input, func);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectManyAsync_ShouldPropagateException_WhenFuncThrowsException()
    {
        // Arrange
        var input = Enumerable.Range(1, 10).Select(_ => _faker.Random.Int()).ToList();
        Func<int, Task<IEnumerable<int>>> func = x => throw new InvalidOperationException();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => 
            await CretNetLinqUtilities.SelectManyAsync(input, func));
    }
}
