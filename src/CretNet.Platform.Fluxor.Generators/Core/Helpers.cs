using System.Collections.Concurrent;

namespace CretNet.Platform.Fluxor.Generators.Core;

public static class Helpers
{
    private static readonly ConcurrentDictionary<string, string> ResourceCache = new();

    public static string GetEmbededResource(string path)
    {
        if (ResourceCache.TryGetValue(path, out var cached))
            return cached;

        var source = string.Empty;

        using var stream = typeof(Helpers).Assembly.GetManifestResourceStream(path);
        if (stream is not null)
        {
            using var streamReader = new StreamReader(stream);
            source = streamReader.ReadToEnd();
        }

        ResourceCache[path] = source;
        return source;
    }
}