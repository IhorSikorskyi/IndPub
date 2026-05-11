using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Notification : BaseEntity
{
    public Notification()
        : base()
    {

    }

    [MinLength(1)]
    [MaxLength(255)]
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    //TODO: Add nullable foreign keys and navigation properties for related entities (e.g., Review, Comment, etc.) to allow notifications for various actions like review likes, comment likes, etc.
    public Guid BookId { get; set; } // Change to nullable
    public Book Book { get; set; } = null!; // Change to nullable

    // Add notification possibility for reviews likes and response, comments likes and response, etc. by adding nullable foreign keys and navigation properties
}