using IndPubBack.Models;

namespace IndPubBack.DTO.Requests;

public class NotificationRequest
{
    public Guid? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public required Guid BookId { get; set; }
    public required string Title { get; set; }
    public Guid? ChapterId { get; set; }
    public string? ChapterName { get; set; }
    public int? ChapterNumber { get; set; }
}

public class ListNotificationRequest
{
    public DateTime? Cursor { get; set; }
    public int PageSize { get; set; }
    public NotificationType? Type { get; set; }
}