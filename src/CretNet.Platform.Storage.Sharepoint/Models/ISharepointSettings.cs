namespace CretNet.Platform.Storage.Sharepoint.Models;

public interface ISharepointSettings
{
    string ClientId { get; set; }
    string ClientSecret { get; set; }
    string RelativeUrl { get; set; }
    string SiteUrl { get; set; }
}
