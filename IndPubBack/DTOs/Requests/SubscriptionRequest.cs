namespace IndPubBack.DTOs.Requests;

public record SubscriptionRequest(Guid AuthorId);

public record SubscriptionListRequest
{
    public bool IsSubscribers { get; init; } = false;
    public DateTime? Cursor { get; init; }
    public int PageSize { get; init; } = 20;
}