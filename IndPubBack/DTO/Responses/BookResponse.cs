using IndPubBack.Models;

namespace IndPubBack.DTO.Responses;

public class BookResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public LanguageCode Language { get; set; } = LanguageCode.En;
    public Status Status { get; set; }
    public string GenreName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string SubcategoryName { get; set; } = null!;
    public double Rating { get; set; }
    public int ChapterCount { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<AuthorResponse> Authors { get; set; } = [];
    public List<ChapterShortResponse> Chapters { get; set; } = [];
}

public class BookShortResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public LanguageCode? Language { get; set; }
    public Status Status { get; set; }
    public int ChapterCount { get; set; }
    public string? GenreName { get; set; }
    public string? CategoryName { get; set; }
    public string? SubcategoryName { get; set; }
    public double? Rating { get; set; }
    public List<AuthorResponse>? Authors { get; set; } = [];
    public List<string>? Tags { get; set; } = [];
}



public class BookmarkShortResponse
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid ChapterId { get; set; }
    public string ChapterTitle { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? BookCoverImageUrl { get; set; }
}

public class GenreResponse
{
    public required string GenreName { get; set; }
    public required string GenreDescription { get; set; }
}

public class AddTagResponse
{
    public Guid BookId { get; set; }
    public string TagName { get; set; } = null!;
    public bool IsAdded { get; set; }
}