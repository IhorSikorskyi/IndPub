namespace IndPubBack.Entities;

public class ReviewLike
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime LikedAt { get; set; }
}