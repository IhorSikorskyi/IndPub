namespace IndPubBack.DTOs.Responses.Book;

public record BookmarkShortResponse
{
    public Guid ChapterId { get; init; }
    public string ChapterTitle { get; init; } = string.Empty;
    public int ChapterNumber { get; init; }
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public string? BookCoverImageUrl { get; init; }
}