namespace IndPubBack.DTO.Requests;

public class SubscriptionRequest
{
    public Guid AuthorId;
}
public class SubscriptionListRequest
{
    public bool IsSubscribers { get; set; } = false;
    public DateTime? Cursor { get; set; }
    public int PageSize { get; set; } = 20;
}