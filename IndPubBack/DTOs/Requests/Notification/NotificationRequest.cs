namespace IndPubBack.DTOs.Requests.Notification;

public record NotificationRequest
{
    public Guid? AuthorId { get; init; }
    public string? AuthorName { get; init; }
    public required Guid BookId { get; init; }
    public required string Title { get; init; }
    public Guid? ChapterId { get; init; }
    public string? ChapterName { get; init; }
    public int? ChapterNumber { get; init; }
}