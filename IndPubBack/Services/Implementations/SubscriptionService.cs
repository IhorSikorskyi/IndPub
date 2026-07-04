using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace IndPubBack.Services.Implementations;

public class SubscriptionService(ISubscriptionRepository subscriptionRepository, IEntityValidationService entityValidationService) : ISubscriptionService
{
    public async Task<UserActivitiesResponse> GetSubscriptionListAsync(Guid userId, SubscriptionListRequest request)
    {
        var data = await subscriptionRepository.GetSubscriptionListAsync(userId, request.IsSubscribers, request.Cursor,
            request.PageSize);

        return new UserActivitiesResponse
        {
            Subscriptions = [..data.Select(s => new SubscriptionShortResponse
            {
                AuthorId = s.AuthorId,
                AuthorLogin = s.Author.Login,
                AuthorProfilePictureUrl = s.Author.ProfilePictureUrl,
            })]
        };
    }

    public async Task<bool> SubscribeAsync(Guid authorId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        await entityValidationService.EnsureUserExistsAsync(authorId);

        if (await IsSubscribedAsync(userId, authorId))
        {
            throw new ConflictException("You subscribe");
        }

        var sub = new Subscription
        {
            AuthorId = authorId,
            UserId = userId
        };

        await subscriptionRepository.AddAsync(sub);

        return true;
    }

    public async Task<bool> UnsubscribeAsync(Guid authorId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        await entityValidationService.EnsureUserExistsAsync(authorId);
        
        var sub = await subscriptionRepository.GetSubscriptionAsync(userId, authorId) ??
                  throw new ConflictException("You not subscribe");

        await subscriptionRepository.UnSubscribedAsync(sub);

        return true;
    }

    private async Task<bool> IsSubscribedAsync(Guid userId, Guid authorId)
    {
        return await subscriptionRepository.IsSubscribedAsync(userId, authorId);
    }

}