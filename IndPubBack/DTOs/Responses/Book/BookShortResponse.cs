using IndPubBack.DTOs.Responses.User;
using IndPubBack.Enums;

namespace IndPubBack.DTOs.Responses.Book;

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