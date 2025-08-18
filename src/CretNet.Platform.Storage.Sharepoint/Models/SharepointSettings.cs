namespace CretNet.Platform.Storage.Sharepoint.Models;

public class SharepointSettings : ISharepointSettings
{
    public const string Section = nameof(SharepointSettings);

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? SiteUrl { get; set; }
    public string? RelativeUrl { get; set; }
}