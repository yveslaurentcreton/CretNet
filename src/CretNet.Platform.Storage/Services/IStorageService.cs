namespace CretNet.Platform.Storage.Services;

public interface IStorageService
{
    string GetBasePath();
    Task<byte[]> GetFile(Guid fileId);
    Task<(Guid id, string newFileName, string checksum)> CreateFile(string fileName, byte[] content, string? path = null);
    Task RemoveFile(Guid fileId);
    Task CreateFolder(string relativeFolderUrl);
}