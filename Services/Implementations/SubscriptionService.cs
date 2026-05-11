using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class SubscriptionService(IConfiguration configuration) : ISubscriptionService
{
    #region Subscription

    //TODO: Implement subscription list retrieval with necessary data aggregation, filtering, and formatting
    public async Task<IList<BookShortResponse>> GetSubscriptionListAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement subscription management with proper authorization checks and data handling
    public async Task<bool> SubscribeAsync(Guid authorId, Guid userId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implement unsubscription management with proper authorization checks and data handling
    public async Task<bool> UnsubscribeAsync(Guid authorId, Guid userId)
    {
        throw new NotImplementedException();
    }

    #endregion
}