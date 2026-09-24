using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetSubscriptionAsync(Guid userId, Guid authorId);
    Task<bool> IsSubscribedAsync(Guid userId, Guid authorId);
    Task<IList<Subscription>> GetSubscriptionListAsync(Guid userId, bool isSubscribers, DateTime? cursor, int pageSize);
    void UnSubscribe(Subscription sub);
    Task SubscribeAsync(Subscription sub);
}