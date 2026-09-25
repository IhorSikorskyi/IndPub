using IndPubBack.DTOs.Responses.Chapter;
using IndPubBack.DTOs.Responses.User;
using IndPubBack.Enums;

namespace IndPubBack.DTOs.Responses.Book;

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