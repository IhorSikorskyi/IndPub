using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class User : BaseEntity
{
    public User() : base()
    {

    }

    [StringLength(50, MinimumLength = 3)]
    public required string Login { get; set; }

    [EmailAddress]
    [MaxLength(254)]
    public required string Email { get; set; }

    [MaxLength(256)]
    public required string PasswordHash { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }
    [MaxLength(2048)]
    public string? ProfilePictureUrl { get; set; }
    public DateTime JoiningDate { get; set; }
    [MaxLength(512)]
    public Roles Role { get; set; } = Roles.User;

    // Exist optional to use virtual collections for lazy loading, but in current implementation we will use eager loading,
    // because of better performance in most cases and requests is not so complex to cause performance issues with eager loading.
    // So, we will use non-virtual collections and initialize them to avoid null reference exceptions.
    public ICollection<LibraryEntry> Entries { get; set; } = new List<LibraryEntry>();
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookLike> BookLikes { get; set; } = new List<BookLike>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public ICollection<BookView> BookViews { get; set; } = new List<BookView>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public enum Roles
{
    User,
    Moderator
}