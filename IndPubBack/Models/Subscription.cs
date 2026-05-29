namespace IndPubBack.Models;

public class Subscription
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}