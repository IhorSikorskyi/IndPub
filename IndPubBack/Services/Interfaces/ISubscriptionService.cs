using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISubscriptionService
{
    Task<UserActivitiesResponse> GetSubscriptionListAsync(Guid userId, SubscriptionListRequest request);
    Task<bool> SubscribeAsync(Guid authorId, Guid userId);
    Task<bool> UnsubscribeAsync(Guid authorId, Guid userId);
}