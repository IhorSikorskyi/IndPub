using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetSubscriptionAsync(Guid userId, Guid authorId);
    Task<bool> IsSubscribedAsync(Guid userId, Guid authorId);
    Task<IList<Subscription>> GetSubscriptionListAsync(Guid userId, bool isSubscribers, DateTime? cursor, int pageSize);
    Task UnSubscribedAsync(Subscription sub);
}