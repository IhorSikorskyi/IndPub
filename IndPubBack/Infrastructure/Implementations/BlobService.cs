using Azure.Storage.Blobs;
using IndPubBack.Infrastructure.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace IndPubBack.Infrastructure.Implementations;

public class BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration) : IBlobService
{
    private readonly string _containerName = configuration["AzureStorage:ContainerName"]
                                             ?? throw new InvalidOperationException("AzureStorage:ContainerName is not configured");

    public async Task<string> UploadBlobAsync(string folder, IFormFile image, Guid entityId)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"{folder.Trim('/')}/{entityId:N}.jpg";
        var blobClient = containerClient.GetBlobClient(blobName);

        await using var inputStream = image.OpenReadStream();
        await using var outputStream = new MemoryStream();

        using var img = await Image.LoadAsync(inputStream);
        await img.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 90 });

        outputStream.Position = 0;
        await blobClient.UploadAsync(outputStream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}