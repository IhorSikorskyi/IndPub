namespace IndPubBack.Infrastructure.Interfaces;

public interface IImageValidationService
{
    bool ValidateImage(IFormFile image, long maxSizeBytes);
}