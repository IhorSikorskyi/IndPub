using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class Book : BaseEntity
{
    [MinLength(1)]
    public required string Title { get; set; }
    [MinLength(3)]
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public required DateTime UpdatedDate { get; set; }

    public double Rating { get; set; }
    
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