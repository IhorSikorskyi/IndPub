namespace IndPubBack.DTOs.Requests.User;

public record LoginRequest
{
    public required string LoginOrEmail { get; init; }

    public required string Password { get; init; }
}