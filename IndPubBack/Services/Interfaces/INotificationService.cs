using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId, ListNotificationRequest request);

    Task<NotificationResponse> CreateNotificationAsync(BookNotificationRequest request, Guid userId);
    Task<NotificationResponse> CreateNotificationAsync(AuthorNotificationRequest request, Guid userId);
    Task DeleteNotificationAsync(Guid userId, Guid notificationId);

    // TODO: Add method for Notifications for Responding to Comments/Reviews
}