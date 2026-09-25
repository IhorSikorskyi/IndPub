namespace IndPubBack.DTOs.Requests.Chapter;

public record ChapterCreateRequest
{
    public string? Title { get; init; }
    public required string Content { get; init; }
    public int? ChapterNumber { get; init; }
}