namespace IndPubBack.DTO.Responses;

public class SubscriptionShortResponse
{
    public Guid AuthorId { get; set; }
    public string AuthorLogin { get; set; } = string.Empty;
    public string? AuthorProfilePictureUrl { get; set; }
}