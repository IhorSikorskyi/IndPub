using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Chapter : BaseEntity
{
    public Chapter()
        : base()
    {

    }

    [MinLength(1)]
    [MaxLength(200)]
    public required string Title { get; set; }
    [MinLength(100)]
    public required string Content { get; set; }
    public required int ChapterNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}