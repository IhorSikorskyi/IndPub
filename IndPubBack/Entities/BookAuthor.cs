namespace IndPubBack.Entities;

public class BookAuthor
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
