using System.Security.Cryptography;
using CretNet.Platform.Storage.Sharepoint.Models;
using Microsoft.SharePoint.Client;
using Uri = System.Uri;

namespace CretNet.Platform.Storage.Sharepoint.Services;

public class SharepointService : ISharepointService
{
    private readonly ISharepointClientContextProvider _clientContextProvider;

    public SharepointService(
        ISharepointClientContextProvider sharepointClientContextProvider)
    {
        _clientContextProvider = sharepointClientContextProvider;
    }

    public Task<byte[]> GetDocument(
        ISharepointSettings sharepointSettings,
        string relativeFileUrl)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var uri = new Uri(sharepointSettings.SiteUrl);
        var decodedUrl = $"{uri.AbsolutePath}/{relativeFileUrl}";
        var encodedRelativeFileUrl = ResourcePath.FromDecodedUrl(decodedUrl);
        var file = clientContext.Web.GetFileByServerRelativePath(encodedRelativeFileUrl);

        return GetDocument(clientContext, file);
    }

    public Task<byte[]> GetDocument(
        ISharepointSettings sharepointSettings,
        Guid documentId)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var file = clientContext.Web.GetFileById(documentId);

        return GetDocument(clientContext, file);
    }

    private Task<byte[]> GetDocument(
        ClientContext clientContext,
        Microsoft.SharePoint.Client.File file)
    {
        var binaryStream = file.OpenBinaryStream();
        clientContext.Load(file);
        clientContext.ExecuteQuery();
        var fromStream = binaryStream.Value;
        var toStream = new MemoryStream();
        fromStream.Seek(0, SeekOrigin.Begin);
        fromStream.CopyTo(toStream);
        toStream.Seek(0, SeekOrigin.Begin);
        toStream.Close();

        return Task.FromResult(toStream.ToArray());
    }

    public Task<(Guid id, string newFileName)> CreateDocument(
        ISharepointSettings sharepointSettings,
        byte[] content,
        string relativeFileUrl)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var fileName = Path.GetFileName(relativeFileUrl);
        var relativeFolderUrl = Path.GetDirectoryName(relativeFileUrl).Replace("\\", "/");
        var uri = new Uri(sharepointSettings.SiteUrl);
        var decodedRelativeFolderUrl = $"{uri.AbsolutePath}/{relativeFolderUrl}";
        var encodedRelativeFolderUrl = ResourcePath.FromDecodedUrl(decodedRelativeFolderUrl);
        var folder = clientContext.Web.GetFolderByServerRelativePath(encodedRelativeFolderUrl);

        return CreateDocument(clientContext, folder, fileName, content);
    }

    public async Task<(Guid id, string newFileName)> CreateDocument(
        ISharepointSettings sharepointSettings,
        Guid folderId,
        string fileName,
        byte[] content,
        string? subFolder = null)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var folder = clientContext.Web.GetFolderById(folderId);

        var subFolderParts = subFolder?.Split('/') ?? Array.Empty<string>();

        foreach (var subFolderPart in subFolderParts)
        {
            if (folder.FolderExists(subFolderPart))
            {
                folder = await folder.ResolveSubFolderAsync(subFolderPart);
            }
            else
            {
                folder = await folder.CreateFolderAsync(subFolderPart);
            }
        }

        return await CreateDocument(clientContext, folder, fileName, content);
    }

    private Task<(Guid id, string newFileName)> CreateDocument(
        ClientContext clientContext,
        Folder folder,
        string fileName,
        byte[] content)
    {
        var files = folder.Files;
        var uploadFile = files.Add(new FileCreationInformation()
        {
            Url = fileName,
            ContentStream = new MemoryStream(content),
            Overwrite = true
        });
        clientContext.Load(uploadFile);
        clientContext.ExecuteQuery();

        // Determine the new filename
        var baseUrl = new Uri(clientContext.Url).GetLeftPart(UriPartial.Authority);
        var newFileName = new Uri(new Uri(baseUrl), uploadFile.ServerRelativeUrl).ToString();

        return Task.FromResult((uploadFile.UniqueId, newFileName));
    }

    public Task UpdateDocument(
        ISharepointSettings sharepointSettings,
        Guid documentId,
        Stream stream)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var file = clientContext.Web.GetFileById(documentId);
        file.SaveBinary(new FileSaveBinaryInformation()
        {
            ContentStream = stream,
        });
        clientContext.Load(file);
        clientContext.ExecuteQuery();

        return Task.CompletedTask;
    }

    public Task RemoveDocument(
        ISharepointSettings sharepointSettings,
        Guid documentId)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);
        var file = clientContext.Web.GetFileById(documentId);
        clientContext.Load(file);
        file.DeleteObject();
        clientContext.ExecuteQuery();

        return Task.CompletedTask;
    }

    public string GenerateChecksum(byte[] content)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(content);
        var checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return checksum;
    }
    
    public async Task<Guid> CreateFolder(
        ISharepointSettings sharepointSettings,
        string relativeFolderUrl)
    {
        using var clientContext = _clientContextProvider.GetClientContext(sharepointSettings);

        var uri = new Uri(sharepointSettings.SiteUrl);
        var decodedFolderUrl = $"{uri.AbsolutePath}/{sharepointSettings.RelativeUrl}";
        var encodedFolderUrl = ResourcePath.FromDecodedUrl(decodedFolderUrl);
        var folder = clientContext.Web.GetFolderByServerRelativePath(encodedFolderUrl);

        clientContext.Load(folder, x => x.Folders);
        clientContext.ExecuteQuery();
        
        var folderParts = relativeFolderUrl?.Split('/') ?? [];

        foreach (var folderPart in folderParts)
        {
            if (folder.FolderExists(folderPart))
            {
                folder = await folder.ResolveSubFolderAsync(folderPart);
            }
            else
            {
                folder = await folder.CreateFolderAsync(folderPart);
            }
        }

        return folder.UniqueId;
    }
}
