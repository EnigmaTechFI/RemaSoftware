using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using RemaSoftware.Domain.Extensions;

namespace RemaSoftware.Domain.Services.Impl;

public class BlobService: IBlobService
{
    private readonly BlobContainerClient _blobContainerClient;
    
    public BlobService(BlobServiceClient blobServiceClient, string blobContainerName)
    {
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName ?? "");
    }
    
    public async Task<BlobElement> GetBlobAsync(string name)
    {
        var blobClient = _blobContainerClient.GetBlobClient(name);
        var downloadBlobInfo = await blobClient.DownloadAsync();
        return new BlobElement(downloadBlobInfo.Value.Content, downloadBlobInfo.Value.ContentType);
    }

    public async Task UploadContentBlobAsync(string content, string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var memoryString = new MemoryStream(bytes);
        await blobClient.UploadAsync(memoryString, new BlobHttpHeaders() {ContentType = fileName.GetContentType()});
    }

    public async Task UploadFromStreamBlobAsync(Stream streamContent, string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(streamContent, new BlobHttpHeaders() {ContentType = fileName.GetContentType()});
    }

    public async Task DeleteBlobAsync(string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task DownloadAndSaveToFile(string localFilePath, string fileName)
    {
        FileStream fileStream = File.OpenWrite(localFilePath);
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        await blobClient.DownloadToAsync(fileStream);
        fileStream.Close();
    }
}

public class OrderImageBlobService : BlobService
{
    public OrderImageBlobService(BlobServiceClient blobServiceClient, string blobContainerName) : base(blobServiceClient, blobContainerName)
    {
    }
}

public class BlobElement
{
    public Stream BlobContent { get; }
    public string BlobContentType { get; }
    public BlobElement(Stream blobContent, string blobContentType)
    {
        BlobContent = blobContent;
        BlobContentType = blobContentType;
    }
}