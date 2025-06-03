using DotNetJet.Utilities;
using Shouldly;

namespace DotNetJet.Tests.Utilities;

public class JetEnumExtensionsTests
{
    [Flags]
    public enum TestFlags
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4
    }

    public enum NonFlagsEnum
    {
        Value1,
        Value2
    }

    [Fact]
    public void ToFlagsCollection_ShouldConvertFlagsToCollection_WhenEnumHasFlags()
    {
        // Arrange
        var enumValue = TestFlags.Flag1 | TestFlags.Flag2;
        var expected = new HashSet<TestFlags> { TestFlags.Flag1, TestFlags.Flag2 };

        // Act
        var result = enumValue.ToFlagsCollection<TestFlags, HashSet<TestFlags>>();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ToFlagsList_ShouldConvertFlagsToList_WhenEnumHasFlags()
    {
        // Arrange
        var enumValue = TestFlags.Flag1 | TestFlags.Flag3;
        var expected = new List<TestFlags> { TestFlags.Flag1, TestFlags.Flag3 };

        // Act
        var result = enumValue.ToFlagsList();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ToFlagsHashSet_ShouldConvertFlagsToHashSet_WhenEnumHasFlags()
    {
        // Arrange
        var enumValue = TestFlags.Flag2 | TestFlags.Flag3;
        var expected = new HashSet<TestFlags> { TestFlags.Flag2, TestFlags.Flag3 };

        // Act
        var result = enumValue.ToFlagsHashSet();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CombineFlags_ShouldCombineFlagsIntoEnum_WhenEnumValuesProvided()
    {
        // Arrange
        var enumValues = new List<TestFlags> { TestFlags.Flag1, TestFlags.Flag2 };

        // Act
        var result = enumValues.CombineFlags();

        // Assert
        result.ShouldBe(TestFlags.Flag1 | TestFlags.Flag2);
    }

    [Fact]
    public void CombineFlags_ShouldReturnNone_WhenEmptyCollectionProvided()
    {
        // Arrange
        var enumValues = new List<TestFlags>();

        // Act
        var result = enumValues.CombineFlags();

        // Assert
        result.ShouldBe(TestFlags.None);
    }

    [Fact]
    public void ToFlagsCollection_ShouldThrowArgumentException_WhenEnumIsNotFlags()
    {
        // Arrange
        var enumValue = NonFlagsEnum.Value1;

        // Act & Assert
        Should.Throw<ArgumentException>(() => enumValue.ToFlagsCollection<NonFlagsEnum, List<NonFlagsEnum>>());
    }
}
