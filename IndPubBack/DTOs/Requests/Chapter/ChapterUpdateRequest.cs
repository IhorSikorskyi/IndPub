namespace IndPubBack.DTOs.Requests.Chapter;

public record ChapterUpdateRequest
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}