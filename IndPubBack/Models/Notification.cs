using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Notification : BaseEntity
{
    public Notification()
        : base()
    {

    }

    [MinLength(1)]
    [MaxLength(255)]
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public NotificationType Type { get; set; } = NotificationType.NewChapter;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid? AuthorId { get; set; }
    public User? Author { get; set; }

    public Guid? ChapterId { get; set; }
    public Chapter? Chapter { get; set; }
}

public enum NotificationType
{
    NewChapter,
    NewBook
}