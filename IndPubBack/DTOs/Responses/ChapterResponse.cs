namespace IndPubBack.DTOs.Responses;

public record ChapterResponse
{
    public Guid BookId { get; init; }
    public Guid ChapterId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public int ChapterNumber { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ChapterShortResponse
{
    public Guid ChapterId { get; init; }
    public required string Title { get; init; }
    public int ChapterNumber { get; init; }
}