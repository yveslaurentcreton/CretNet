namespace CretNet.Platform.Blazor.Services;

public interface IServerTimeProvider
{
    Task<DateTimeOffset> GetServerTimeAsync();
}