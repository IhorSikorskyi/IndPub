namespace IndPubBack.DTOs.Requests.Book;

public record ChapterCreateWithBookRequest
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}