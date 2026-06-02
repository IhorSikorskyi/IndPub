using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface INotificationRepository: IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync
        (Guid userId, DateTime? cursor, int pageSize, NotificationType? type);

    Task RemoveOldNotificationsAsync(CancellationToken cancellationToken = default);
}