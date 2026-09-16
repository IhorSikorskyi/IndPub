using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Notification : BaseEntity
{
    [MinLength(1)]
    public required string Message { get; set; }
    public NotificationType Type { get; set; }

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