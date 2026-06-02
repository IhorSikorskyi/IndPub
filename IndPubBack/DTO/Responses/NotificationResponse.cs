using IndPubBack.Models;

namespace IndPubBack.DTO.Responses;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string? Author { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? ChapterTitle { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public NotificationType Type { get; set; }
}