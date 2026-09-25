using IndPubBack.DTOs.Requests.Notification;
using IndPubBack.DTOs.Responses.Notification;

namespace IndPubBack.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId, ListNotificationRequest request);

    Task<NotificationResponse> CreateNotificationAsync(NotificationRequest request, Guid userId);
    Task DeleteNotificationAsync(Guid userId, Guid notificationId);

    // TODO: Add method for Notifications for Responding to Comments/Reviews
}