using IndPubBack.Entities;
using IndPubBack.Enums;

namespace IndPubBack.DTOs.Requests;

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

public record ListNotificationRequest
{
    public DateTime? Cursor { get; init; }
    public required int PageSize { get; init; }
    public NotificationType? Type { get; init; }
}