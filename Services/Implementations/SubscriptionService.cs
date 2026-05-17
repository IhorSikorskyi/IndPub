using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class SubscriptionService(ISubscriptionRepository subscriptionRepository, IUserRepository userRepository) : ISubscriptionService
{
    public async Task<UserActivitiesResponse> GetSubscriptionListAsync(Guid userId, SubscriptionListRequest request)
    {
        var data = await subscriptionRepository.GetSubscriptionListAsync(userId, request.IsSubscribers, request.Cursor,
            request.PageSize);

        return new UserActivitiesResponse
        {
            Subscriptions = data.Select(s => new SubscriptionShortResponse
            {
                AuthorId = s.AuthorId,
                AuthorLogin = s.Author.Login,
                AuthorProfilePictureUrl = s.Author.ProfilePictureUrl,
            }).ToList()
        };
    }

    public async Task<bool> SubscribeAsync(Guid authorId, Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if (!await userRepository.IsExistAsync(authorId))
        {
            throw new NotFoundException("Author not found");
        }

        if (await IsSubscribedAsync(userId, authorId))
        {
            throw new ConflictException("You already subscribe");
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
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if (!await userRepository.IsExistAsync(authorId))
        {
            throw new NotFoundException("Author not found");
        }

        if (!await IsSubscribedAsync(userId, authorId))
        {
            throw new ConflictException("You already not subscribe");
        }

        await subscriptionRepository.UnSubscribedAsync(userId, authorId);

        return true;
    }

    public async Task<bool> IsSubscribedAsync(Guid userId, Guid authorId)
    {
        return await subscriptionRepository.IsSubscribedAsync(userId, authorId);
    }

}