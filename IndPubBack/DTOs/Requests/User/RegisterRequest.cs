using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests.User;

public record RegisterRequest
{
    public required string Login { get; init; }

    public required string Email { get; init; }

    [MinLength(1)]
    public required string Password { get; init; }

    public required string ConfirmPassword { get; init; }
}