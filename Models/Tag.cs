using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndPubBack.Models;

public class Tag : BaseEntity
{
    public Tag()
        : base()
    {

    }

    public string Name { get; set; } = null!;

    public ICollection<BookTag>? BookTags { get; set; }
}

public class BookTag
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
