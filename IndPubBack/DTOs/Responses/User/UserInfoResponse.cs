namespace IndPubBack.DTOs.Responses.User;

public record UserInfoResponse
{
    public string Login { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Bio { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public DateTime? JoiningDate { get; init; }
    public int? SubscribersCount { get; init; }
}