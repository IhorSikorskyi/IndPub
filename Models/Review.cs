using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Review : BaseEntity
{
    public Review()
        : base()
    {

    }

    [Range(0.5, 5.0)]
    public required double Rating { get; set; }
    [MinLength(1)]
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public required User User { get; set; } = null!;

    public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class ReviewLike
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}