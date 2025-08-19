namespace CretNet.Platform.Helpers;

public static class PathHelper
{
    public static string CombineSharepointPaths(params string?[] paths)
    {
        return string.Join('/', paths.Where(path => !string.IsNullOrWhiteSpace(path)).Select(x => x!.Trim('/', '\\')));
    }
}