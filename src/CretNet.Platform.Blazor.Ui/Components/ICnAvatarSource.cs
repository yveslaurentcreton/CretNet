namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Optional photo source behind <see cref="CnAvatar"/>. A host that stores
/// profile photos (HCMT's AvatarService) registers an implementation; hosts
/// without one register nothing and every avatar renders initials. The
/// component resolves this through <c>IServiceProvider.GetService</c> —
/// never a hard <c>[Inject]</c>, which throws for unregistered services
/// regardless of nullability.
/// </summary>
public interface ICnAvatarSource
{
    /// <summary>Photo URL for the party, or null to fall back to initials.</summary>
    Task<string?> GetPartyAvatarUrlAsync(Guid partyId);

    /// <summary>Raised when a photo changes so visible avatars refresh.</summary>
    event Action? Changed;
}
