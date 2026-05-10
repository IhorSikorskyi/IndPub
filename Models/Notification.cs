using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndPubBack.Models;

public class Notification : BaseEntity
{
    public Notification()
        : base()
    {

    }

    [MinLength(1)] [MaxLength(255)]
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}