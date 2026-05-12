namespace IndPubBack.Models;

public class BookView
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}