using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class SubscriptionRepository(Connected dbContext) : Repository<Subscription> (dbContext), ISubscriptionRepository
{
    public async Task<Subscription?> GetSubscriptionAsync(Guid userId, Guid authorId)
    {
        return await dbContext.Subscriptions
            .Where(s => s.UserId == userId && s.AuthorId == authorId)
            .Include(s => s.Author)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsSubscribedAsync(Guid userId, Guid authorId)
    {
        return await dbContext.Subscriptions
            .AnyAsync(s => s.UserId == userId && s.AuthorId == authorId);
    }

    public async Task<IList<Subscription>> GetSubscriptionListAsync(Guid userId, bool isSubscribers, DateTime? cursor, int pageSize)
    {
        IQueryable<Subscription> query = isSubscribers
            ? dbContext.Subscriptions.Where(s => s.AuthorId == userId).Include(s => s.User)
            : dbContext.Subscriptions.Where(s => s.UserId == userId).Include(s => s.Author);

        if (cursor != null)
        {
            query = query.Where(s => s.SubscribedAt < cursor);
        }

        return await query
            .OrderByDescending(s => s.SubscribedAt)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> UnSubscribedAsync(Guid userId, Guid authorId)
    {
        var entity = await GetSubscriptionAsync(userId, authorId) ??
                     throw new NotFoundException("Subscription not found");

        dbContext.Subscriptions.Remove(entity);
        await dbContext.SaveChangesAsync();

        return true;
    }
}