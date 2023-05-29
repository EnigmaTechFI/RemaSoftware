using RemaSoftware.Domain.Services.Impl;

namespace RemaSoftware.Domain.Services;

public interface IBlobService
{
    Task<BlobElement> GetBlobAsync(string name);
    Task UploadContentBlobAsync(string content, string fileName);
    Task UploadFromStreamBlobAsync(Stream streamContent, string fileName);
    Task DeleteBlobAsync(string fileName);
    Task DownloadAndSaveToFile(string localFilePath, string filename);
}