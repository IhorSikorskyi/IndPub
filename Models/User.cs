using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Models;

public class User : BaseEntity
{
    public User() : base()
    {

    }

    [StringLength(50, MinimumLength = 3)]
    public required string Login { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; } = DateTime.UtcNow.AddDays(7);

    public ICollection<LibraryEntry> Entries { get; set; } = new List<LibraryEntry>();
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookLike> BookLikes { get; set; } = new List<BookLike>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}