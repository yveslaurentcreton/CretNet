namespace CretNet.Platform.Fluxor.Generators.Core;

public static class StringExtensions
{
    public static string RemoveEnd(this string source, string value)
    {
        if (!source.EndsWith(value))
            return source;

        return source.Substring(0, source.Length - value.Length);
    }
    
    public static (string, string?) SplitGenericType(this string input)
    {
        // Find the index of the first '<' and the last '>'
        int startIndex = input.IndexOf('<');
        int endIndex = input.LastIndexOf('>');
        
        // Check if the input is a generic type
        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            // Extract the part before '<' and the part between '<' and '>'
            string baseType = input.Substring(0, startIndex) + "`1";
            string genericArgument = input.Substring(startIndex + 1, endIndex - startIndex - 1);
            return (baseType, genericArgument);
        }
        else
        {
            // Return the input as the base type and an empty string as the generic argument
            return (input, null);
        }
    }
}