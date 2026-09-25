namespace IndPubBack.DTOs.Responses.Chapter;

public record ChapterShortResponse
{
    public Guid ChapterId { get; init; }
    public required string Title { get; init; }
    public int ChapterNumber { get; init; }
}