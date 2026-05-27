namespace IndPubBack.Models;

public class Category : BaseEntity
{
    public Category()
        : base()
    {

    }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Subcategory : BaseEntity
{
    public Subcategory()
        : base()
    {

    }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}