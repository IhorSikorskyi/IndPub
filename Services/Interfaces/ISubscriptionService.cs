using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISubscriptionService
{
    Task<IList<BookShortResponse>> GetSubscriptionListAsync(Guid userId);
    Task<bool> SubscribeAsync(Guid authorId, Guid userId);
    Task<bool> UnsubscribeAsync(Guid authorId, Guid userId);
}