using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Comment : BaseEntity
{
    public Comment()
        : base()
    {

    }

    [Required]
    [MinLength(1)]
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ChapterId { get; set; }
    public Chapter? Chapter { get; set; }

    public Guid? ReviewId { get; set; }
    public Review? Review { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
}

public class CommentLike
{
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}