using IndPubBack.Entities;
using IndPubBack.Enums;

namespace IndPubBack.DTOs.Responses;

public record NotificationResponse
{
    public Guid Id { get; init; }
    public string? Author { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public string? ChapterTitle { get; init; }
    public required string Message { get; init; }
    public DateTime CreatedAt { get; init; }
    public NotificationType Type { get; init; }
}