namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Shared photo cache and refresh notifications. Hosts implement the lookup using
/// their own API (uploaded profile photos, Office 365, or another photo provider).
/// Register the same scoped instance as <see cref="ICnAvatarSource"/>.
/// </summary>
public abstract class CnAvatarSource : ICnAvatarSource
{
    private readonly Dictionary<Guid, Lazy<Task<string?>>> _photos = [];
    private readonly object _gate = new();

    public event Action? Changed;

    /// <summary>Coalesces concurrent lookups and caches missing photos.</summary>
    public async Task<string?> GetPartyAvatarUrlAsync(Guid partyId)
    {
        Lazy<Task<string?>> photo;
        lock (_gate)
        {
            if (!_photos.TryGetValue(partyId, out photo!))
            {
                photo = new(() => LoadPartyAvatarUrlAsync(partyId));
                _photos[partyId] = photo;
            }
        }

        try
        {
            return await photo.Value;
        }
        catch
        {
            // A failed transport is retryable; do not evict a newer upload.
            lock (_gate)
            {
                if (_photos.TryGetValue(partyId, out var current) && ReferenceEquals(current, photo))
                    _photos.Remove(partyId);
            }
            throw;
        }
    }

    /// <summary>The host resolves an image URL or data URL; null means no photo.</summary>
    protected abstract Task<string?> LoadPartyAvatarUrlAsync(Guid partyId);

    /// <summary>Updates visible avatars immediately after the host saves a photo.</summary>
    protected void SetPartyAvatarUrl(Guid partyId, string? url)
    {
        lock (_gate)
            _photos[partyId] = new(() => Task.FromResult(url));
        Changed?.Invoke();
    }

    /// <summary>Forgets a cached photo and asks visible avatars to load it again.</summary>
    public void Invalidate(Guid partyId)
    {
        lock (_gate)
            _photos.Remove(partyId);
        Changed?.Invoke();
    }
}
