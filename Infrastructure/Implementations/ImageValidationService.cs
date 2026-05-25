using IndPubBack.Infrastructure.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class ImageValidationService : IImageValidationService
{
    public bool ValidateImage(IFormFile image, long maxSizeBytes)
    {
        if (image.Length == 0 || image.Length > maxSizeBytes)
        {
            return false;
        }

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };

        var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/jpg"
        };

        var extension = Path.GetExtension(image.FileName);

        return !string.IsNullOrWhiteSpace(extension)
               && allowedExtensions.Contains(extension)
               && !string.IsNullOrWhiteSpace(image.ContentType)
               && allowedContentTypes.Contains(image.ContentType);
    }
}