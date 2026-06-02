using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class NotificationRepository(Connected dbContext) : Repository<Notification>(dbContext), INotificationRepository
{
    private readonly DateTime _lastNotificationDate = DateTime.UtcNow.AddMonths(-2);

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync
        (Guid userId, DateTime? cursor, int pageSize, NotificationType? type)
    {
        var query = dbContext.Notifications
            .Include(n => n.Book)
            .Include(n => n.Chapter)
            .AsQueryable();

        query = query.Where(n => n.UserId == userId);

        if (cursor.HasValue)
        {
            query = query.Where(n => n.CreatedAt < cursor.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(n => n.Type == type.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task RemoveOldNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var oldNotifications = await dbContext.Notifications
            .Where(n => n.CreatedAt < _lastNotificationDate)
            .ToListAsync(cancellationToken);
        dbContext.Notifications.RemoveRange(oldNotifications);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}