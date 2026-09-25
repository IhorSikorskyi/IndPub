namespace IndPubBack.DTOs.Responses.Subscription;

public record SubscriptionShortResponse
{
    public Guid AuthorId { get; init; }
    public string AuthorLogin { get; init; } = string.Empty;
    public string? AuthorProfilePictureUrl { get; init; }
}