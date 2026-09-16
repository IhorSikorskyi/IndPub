using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Tag : BaseEntity
{
    [MinLength(1)]
    public required string Name { get; set; } = null!;

    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
}
