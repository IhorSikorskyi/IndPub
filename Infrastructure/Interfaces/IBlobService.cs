namespace IndPubBack.Infrastructure.Interfaces;

public interface IBlobService
{
    Task<string> UploadBlobAsync(string folder, IFormFile image, Guid entityId);
}