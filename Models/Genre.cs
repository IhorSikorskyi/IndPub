using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndPubBack.Models;

public class Genre : BaseEntity
{
    public Genre()
        : base()
    {

    }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}