namespace IndPubBack.Entities;

public class Subcategory : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}