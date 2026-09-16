using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class User : BaseEntity
{
    [MinLength(3)]
    public required string Login { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public Roles Role { get; set; } = Roles.User;

    public ICollection<LibraryEntry> LibraryEntries { get; set; } = new List<LibraryEntry>();
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookLike> BookLikes { get; set; } = new List<BookLike>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Notification> AuthoredNotifications { get; set; } = new List<Notification>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public ICollection<BookView> BookViews { get; set; } = new List<BookView>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public enum Roles
{
    User,
    Moderator
}