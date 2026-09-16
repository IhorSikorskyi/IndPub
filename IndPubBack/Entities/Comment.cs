using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Comment : BaseEntity
{
    [Required]
    [MinLength(1)]
    public required string Text { get; set; }

    public Guid? ChapterId { get; set; }
    public Chapter? Chapter { get; set; }

    public Guid? ReviewId { get; set; }
    public Review? Review { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
}