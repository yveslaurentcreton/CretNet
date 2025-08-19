namespace CretNet.Platform.Blazor.Services;

public interface ITimeService
{
    Task InitializeAsync();
    
    DateTimeOffset GetServerTime();
    DateTimeOffset GetClientTime();
    
    DateTimeOffset ConvertToLocalDateTimeOffset(DateTime localTime);
    DateTimeOffset? ConvertToLocalDateTimeOffset(DateTime? localTime);
    DateTimeOffset ConvertToLocalDateTimeOffset(DateTimeOffset dateTimeOffset);
    DateTimeOffset? ConvertToLocalDateTimeOffset(DateTimeOffset? dateTimeOffset);
}