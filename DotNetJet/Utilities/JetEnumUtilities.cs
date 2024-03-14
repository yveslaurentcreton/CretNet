namespace DotNetJet.Utilities;

/// <summary>
/// Provides utility methods for operations related to Enum types, such as flag manipulation and collection conversion.
/// </summary>
public static class JetEnumExtensions
{
    /// <summary>
    /// Extension method to convert an Enum value with flags into a collection of its individual flags.
    /// </summary>
    /// <param name="enumValue">The Enum value to convert.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <typeparam name="TCollection">The type of the collection to populate with Enum flags.</typeparam>
    /// <returns>A collection of Enum flags.</returns>
    public static TCollection ToFlagsCollection<TEnum, TCollection>(this TEnum enumValue)
        where TEnum : struct, Enum
        where TCollection : ICollection<TEnum>, new()
    {
        return GetFlagsAsCollection<TEnum, TCollection>(enumValue);
    }
    
    /// <summary>
    /// Converts an Enum value with flags into a List of its individual flags.
    /// </summary>
    /// <param name="enumValue">The Enum value to convert.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <returns>A List containing individual Enum flags.</returns>
    public static List<TEnum> ToFlagsList<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return GetFlagsAsCollection<TEnum, List<TEnum>>(enumValue);
    }

    /// <summary>
    /// Converts an Enum value with flags into a HashSet of its individual flags.
    /// </summary>
    /// <param name="enumValue">The Enum value to convert.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <returns>A HashSet containing individual Enum flags.</returns>
    public static HashSet<TEnum> ToFlagsHashSet<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return GetFlagsAsCollection<TEnum, HashSet<TEnum>>(enumValue);
    }

    /// <summary>
    /// Extension method to combine a collection of Enum values into a single Enum value with flags.
    /// </summary>
    /// <param name="enumValues">The collection of Enum values to combine.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <returns>An Enum value that represents the bitwise OR of the provided Enum values.</returns>
    public static TEnum CombineFlags<TEnum>(this IEnumerable<TEnum> enumValues)
        where TEnum : struct, Enum
    {
        return CombineEnumFlags(enumValues);
    }

    /// <summary>
    /// Converts an Enum value with flags into a collection of its individual flags.
    /// </summary>
    /// <param name="value">The Enum value to convert.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <typeparam name="TCollection">The type of the collection to populate with Enum flags.</typeparam>
    /// <returns>A collection of Enum flags.</returns>
    public static TCollection GetFlagsAsCollection<TEnum, TCollection>(TEnum value)
        where TEnum : struct, Enum
        where TCollection : ICollection<TEnum>, new()
    {
        EnsureIsFlagsEnum<TEnum>();

        var enumValues = Enum.GetValues(typeof(TEnum)).OfType<TEnum>();
        var selectedValues = new TCollection();
        foreach (var enumValue in enumValues)
        {
            // Skip the 'None' value if it's defined as 0
            if (Convert.ToInt64(enumValue) == 0) continue;
        
            var valueAsLong = Convert.ToInt64(value);
            var enumValueAsLong = Convert.ToInt64(enumValue);

            if ((valueAsLong & enumValueAsLong) == enumValueAsLong)
            {
                selectedValues.Add(enumValue);
            }
        }
        return selectedValues;
    }

    /// <summary>
    /// Combines a collection of Enum values into a single Enum value with flags.
    /// </summary>
    /// <param name="values">The collection of Enum values to combine.</param>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <returns>An Enum value that represents the bitwise OR of the provided Enum values.</returns>
    public static TEnum CombineEnumFlags<TEnum>(IEnumerable<TEnum> values)
        where TEnum : struct, Enum
    {
        EnsureIsFlagsEnum<TEnum>();

        long result = 0;
        foreach (var item in values)
        {
            var itemValue = Convert.ToInt64(item);
            result |= itemValue;
        }
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }

    /// <summary>
    /// Ensures the Enum type is a flags enum. Throws an ArgumentException if the Enum type does not have the FlagsAttribute.
    /// </summary>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    private static void EnsureIsFlagsEnum<TEnum>() where TEnum : struct
    {
        if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
        {
            throw new ArgumentException($"The type '{typeof(TEnum).Name}' is not a bit field.");
        }
    }
}
