using CretNet.Platform.Storage.Sharepoint.Models;
using Microsoft.SharePoint.Client;

namespace CretNet.Platform.Storage.Sharepoint.Services;

public interface ISharepointClientContextProvider
{
    ClientContext GetClientContext(ISharepointSettings sharepointSettings);
}