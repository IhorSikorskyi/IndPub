namespace IndPubBack.Infrastructure.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile image, string folderConfigKey, Guid entityId, long maxFileSize);
}