using CretNet.Platform.Storage.Services;
using CretNet.Platform.Storage.Sharepoint.Helpers;
using CretNet.Platform.Storage.Sharepoint.Models;
using Microsoft.Extensions.Options;

namespace CretNet.Platform.Storage.Sharepoint.Services;

public class StorageService : IStorageService
{
    private readonly SharepointSettings _documentSharepointSettings;
    private readonly ISharepointService _sharepointService;

    public StorageService(IOptions<SharepointSettings> documentSharepointSettings, ISharepointService service)
    {
        _documentSharepointSettings = documentSharepointSettings.Value;
        _sharepointService = service;
    }

    public string GetBasePath()
    {
        var url = $"{_documentSharepointSettings.SiteUrl}/{_documentSharepointSettings.RelativeUrl}";
        
        return url;
    }

    public Task<byte[]> GetFile(Guid fileId)
    {
        return _sharepointService.GetDocument(_documentSharepointSettings, fileId);
    }

    public async Task<(Guid id, string newFileName, string checksum)> CreateFile(string fileName, byte[] content, string? path = null)
    {
        var documentPath = PathHelper.CombineSharepointPaths(_documentSharepointSettings.RelativeUrl, path, fileName);
        
        var (id, newFileName) = await _sharepointService.CreateDocument(_documentSharepointSettings, content, documentPath);
        var checksum = _sharepointService.GenerateChecksum(content);

        return (id, newFileName, checksum);
    }

    public Task RemoveFile(Guid fileId)
    {
        return _sharepointService.RemoveDocument(_documentSharepointSettings, fileId);
    }
    
    public Task CreateFolder(string relativeFolderUrl)
    {
        return _sharepointService.CreateFolder(_documentSharepointSettings, relativeFolderUrl);
    }
}
