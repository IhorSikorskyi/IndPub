using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Genre : BaseEntity
{
    public Genre()
        : base()
    {

    }

    [MinLength(1)]
    public required string Name { get; set; }
    [MinLength(1)]
    public required string Description { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}