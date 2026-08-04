namespace IndPubBack.Models;

public class LibraryEntry
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public LibraryBookStatus Status { get; set; } = LibraryBookStatus.Planned;
}

public enum LibraryBookStatus
{
    Reading,
    Completed,
    Planned,
    OnHold,
    Dropped
}