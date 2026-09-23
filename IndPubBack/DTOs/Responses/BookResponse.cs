using IndPubBack.Enums;

namespace IndPubBack.DTOs.Responses;

public record BookResponse
{
    public Guid BookId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public DateTime PublishedDate { get; init; }
    public DateTime UpdatedDate { get; init; }
    public LanguageCode Language { get; init; } = LanguageCode.En;
    public Status Status { get; init; }
    public required string GenreName { get; init; }
    public required string CategoryName { get; init; }
    public required string SubcategoryName { get; init; }
    public double Rating { get; init; }
    public int ChapterCount { get; init; }
    public List<string> Tags { get; init; } = [];
    public List<AuthorResponse> Authors { get; init; } = [];
    public List<ChapterShortResponse> Chapters { get; init; } = [];
}

public record BookShortResponse
{
    public Guid BookId { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public LanguageCode? Language { get; init; }
    public Status Status { get; init; }
    public int ChapterCount { get; init; }
    public string? GenreName { get; init; }
    public string? CategoryName { get; init; }
    public string? SubcategoryName { get; init; }
    public double? Rating { get; init; }
    public List<AuthorResponse>? Authors { get; init; } = [];
    public List<string>? Tags { get; init; } = [];
}

public record BookmarkShortResponse
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid ChapterId { get; init; }
    public string ChapterTitle { get; init; } = string.Empty;
    public int ChapterNumber { get; init; }
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public string? BookCoverImageUrl { get; init; }
}

public record GenreResponse
{
    public required string GenreName { get; init; }
    public required string GenreDescription { get; init; }
}

public record AddTagResponse
{
    public Guid BookId { get; init; }
    public required string TagName { get; init; }
    public bool IsAdded { get; init; }
}