using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Book : BaseEntity
{
    public Book()
        : base()
    {

    }

    [StringLength(255, MinimumLength = 1)]
    public required string Title { get; set; }
    [StringLength(5000, MinimumLength = 3)]
    public string? Description { get; set; }
    [MaxLength(2048)]
    public string? CoverImageUrl { get; set; }
    public required DateTime PublishedDate { get; set; }
    public required DateTime UpdatedDate { get; set; }

    public double Rating { get; set; } = 0;
    
    public LanguageCode Language { get; set; } = LanguageCode.En;
    public Status Status { get; set; } = Status.Ongoing;

    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid SubcategoryId { get; set; }
    public Subcategory Subcategory { get; set; } = null!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookLike> BookLikes { get; set; } = new List<BookLike>();
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
    public ICollection<LibraryEntry> LibraryEntries { get; set; } = new List<LibraryEntry>();
    public ICollection<BookView> BookViews { get; set; } = new List<BookView>();
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

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
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

public enum LanguageCode
{
    En,
    Es,
    Fr,
    De,
    It,
    Pt,
    Ru,
    Ja,
    Zh
}