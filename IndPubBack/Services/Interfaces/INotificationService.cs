using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;

namespace IndPubBack.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId, ListNotificationRequest request);

    Task<NotificationResponse> CreateNotificationAsync(NotificationRequest request, Guid userId);
    Task DeleteNotificationAsync(Guid userId, Guid notificationId);

    // TODO: Add method for Notifications for Responding to Comments/Reviews
}