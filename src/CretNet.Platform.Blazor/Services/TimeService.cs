using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Services;

public class TimeService : ITimeService
{
    private readonly IServerTimeProvider _serverTimeProvider;
    private readonly IJSRuntime _jsRuntime;
    
    private TimeSpan _timeOffset = TimeSpan.Zero;
    private bool _isSynced;
    private bool _isInitialized;

    public TimeZoneInfo BrowserTimeZone { get; private set; }

    public TimeService(IServerTimeProvider serverTimeProvider, IJSRuntime jsRuntime)
    {
        _serverTimeProvider = serverTimeProvider;
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;
        
        var serverTimeSyncTask = SyncTimeOffsetAsync();
        var browserTimezoneTask = InitBrowserTimezoneAsync();
        
        await Task.WhenAll(serverTimeSyncTask, browserTimezoneTask);
        
        _isInitialized = true;
    }
    
    private async Task SyncTimeOffsetAsync()
    {
        var startServerTimeRequest = DateTimeOffset.Now;
        var serverTime = await _serverTimeProvider.GetServerTimeAsync();
        var stopServerTimeRequest = DateTimeOffset.Now;
        var serverTimeRequestRoundTrip = stopServerTimeRequest - startServerTimeRequest;
        var clientTime = GetClientTime();
        _timeOffset = serverTime - clientTime + serverTimeRequestRoundTrip / 2;
        _isSynced = true;
    }

    private async Task InitBrowserTimezoneAsync()
    {
        var timezone = await _jsRuntime.InvokeAsync<string>("eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
        BrowserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
    }

    public DateTimeOffset GetServerTime()
    {
        if (!_isSynced)
            throw new InvalidOperationException("Time offset is not synced. Call SyncTimeOffsetAsync() first or use GetServerTimeAsync()");
        
        return GetClientTime().Add(_timeOffset);
    }
    
    public DateTimeOffset GetClientTime()
    {
        return DateTimeOffset.Now;
    }

    public DateTimeOffset ConvertToLocalDateTimeOffset(DateTime localTime)
    {
        var unspecifiedLocalTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
        var offset = BrowserTimeZone.GetUtcOffset(unspecifiedLocalTime);
        return new DateTimeOffset(unspecifiedLocalTime, offset);
    }

    public DateTimeOffset? ConvertToLocalDateTimeOffset(DateTime? localTime)
    {
        if (localTime == null)
            return null;

        return ConvertToLocalDateTimeOffset(localTime.Value);
    }
    
    public DateTimeOffset ConvertToLocalDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return TimeZoneInfo.ConvertTime(dateTimeOffset, BrowserTimeZone);
    }

    public DateTimeOffset? ConvertToLocalDateTimeOffset(DateTimeOffset? dateTimeOffset)
    {
        if (dateTimeOffset == null)
            return null;

        return ConvertToLocalDateTimeOffset(dateTimeOffset.Value);
    }
}
