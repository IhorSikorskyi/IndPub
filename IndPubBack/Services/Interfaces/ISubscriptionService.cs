using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISubscriptionService
{
    Task<UserActivitiesResponse> GetSubscriptionListAsync(Guid userId, SubscriptionListRequest request);
    Task<bool> SubscribeAsync(Guid authorId, Guid userId);
    Task<bool> UnsubscribeAsync(Guid authorId, Guid userId);
}