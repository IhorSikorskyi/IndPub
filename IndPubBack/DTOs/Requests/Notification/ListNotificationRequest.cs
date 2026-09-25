using IndPubBack.Enums;

namespace IndPubBack.DTOs.Requests.Notification;

public record ListNotificationRequest
{
    public DateTime? Cursor { get; init; }
    public required int PageSize { get; init; }
    public NotificationType? Type { get; init; }
}