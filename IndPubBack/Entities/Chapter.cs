using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Chapter : BaseEntity
{
    [MinLength(1)]
    public required string Title { get; set; }
    [MinLength(100)]
    public required string Content { get; set; }
    public required int ChapterNumber { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}