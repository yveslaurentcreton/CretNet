using CretNet.Platform.Storage.Sharepoint.Models;

namespace CretNet.Platform.Storage.Sharepoint.Services;

public interface ISharepointService
{
    Task<(Guid id, string newFileName)> CreateDocument(ISharepointSettings sharepointSettings, byte[] content, string relativeFileUrl);
    Task<(Guid id, string newFileName)> CreateDocument(ISharepointSettings sharepointSettings, Guid folderId, string fileName, byte[] content, string? subFolder = null); 
    Task<byte[]> GetDocument(ISharepointSettings sharepointSettings, string relativeFileUrl);
    Task<byte[]> GetDocument(ISharepointSettings sharepointSettings, Guid documentId);
    Task UpdateDocument(ISharepointSettings sharepointSettings, Guid documentId, Stream stream);
    Task RemoveDocument(ISharepointSettings sharepointSettings, Guid documentId);
    string GenerateChecksum(byte[] content);

    Task<Guid> CreateFolder(
        ISharepointSettings sharepointSettings,
        string relativeFolderUrl);
}