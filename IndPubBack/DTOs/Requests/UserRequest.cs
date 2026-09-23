using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests;

public record RegisterRequest
{
    public required string Login { get; init; }

    public required string Email { get; init; }

    [MinLength(1)]
    public required string Password { get; init; }

    public required string ConfirmPassword { get; init; }
}

public record LoginRequest
{
    public required string LoginOrEmail { get; init; }

    public required string Password { get; init; }
}

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