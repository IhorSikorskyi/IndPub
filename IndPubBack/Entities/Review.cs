using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Review : BaseEntity
{
    [Range(0.5, 5.0)]
    public required double Rating { get; set; }
    [MinLength(1)]
    [MaxLength(10000)]
    public required string Text { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}