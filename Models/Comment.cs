using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndPubBack.Models;

public class Comment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Text { get; set; } = null!;
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