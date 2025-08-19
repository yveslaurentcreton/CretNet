using CretNet.Platform.Storage.Sharepoint.Models;
using Microsoft.SharePoint.Client;
using PnP.Framework;

namespace CretNet.Platform.Storage.Sharepoint.Services;

public class SharepointClientContextProvider : ISharepointClientContextProvider
{
    public ClientContext GetClientContext(ISharepointSettings sharepointSettings)
    {
        return new AuthenticationManager().GetACSAppOnlyContext(
            sharepointSettings.SiteUrl,
            sharepointSettings.ClientId,
            sharepointSettings.ClientSecret);
    }
}
