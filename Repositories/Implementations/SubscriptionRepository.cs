using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations;

public class SubscriptionRepository(Connected dbContext) : Repository<Subscription> (dbContext), ISubscriptionRepository
{
    
}