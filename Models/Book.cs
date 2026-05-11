using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Book : BaseEntity
{
    public Book()
        : base()
    {

    }

    [MinLength(1)]
    public required string Title { get; set; }
    [MinLength(1)]
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public required DateTime PublishedDate { get; set; }
    public required DateTime UpdatedDate { get; set; }
    public required int ChapterCount { get; set; }
    public required string Language { get; set; } = "en";
    public required Status Status { get; set; } = Status.Ongoing;

    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = null!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookLike> BookLikes { get; set; } = new List<BookLike>();
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
    public ICollection<LibraryEntry> LibraryEntries { get; set; } = new List<LibraryEntry>();
}

public class BookAuthor
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

public class BookLike
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

public class Bookmark
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;
}

public enum Status
{
    Ongoing,
    Completed,
    Dropped
}