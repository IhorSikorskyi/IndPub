namespace IndPubBack.DTOs.Requests;

public record ChapterCreateRequest
{
    public string? Title { get; init; }
    public required string Content { get; init; }
    public int? ChapterNumber { get; init; }
}

public record ChapterUpdateRequest
{
    public string? Title { get; init; }
    public string? Content { get; init; }
}