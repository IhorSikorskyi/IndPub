using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests.User;

public record UpdateProfileRequest
{
    public string? Login { get; init; }

    public string? Email { get; init; }

    public string? CurrentPassword { get; init; }

    [MinLength(1)]
    public string? NewPassword { get; init; }

    public string? ConfirmNewPassword { get; init; }

    public string? Bio { get; init; }

    public IFormFile? ProfilePicture { get; init; }
}