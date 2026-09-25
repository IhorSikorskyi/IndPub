namespace IndPubBack.DTOs.Responses.User;

public record AuthorResponse
{
    public Guid Id { get; init; }
    public required string Login { get; init; }
    public string? ProfilePictureUrl { get; init; }
}