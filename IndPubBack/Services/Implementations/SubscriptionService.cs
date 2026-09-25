using IndPubBack.DTOs.Requests.Subscription;
using IndPubBack.DTOs.Responses.Subscription;
using IndPubBack.DTOs.Responses.User;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    IEntityValidationService entityValidationService) : ISubscriptionService
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
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        _ = await entityValidationService.IsUserExistsAsync(authorId) ? true
            : throw new NotFoundException("Author not found");

        if (await IsSubscribedAsync(userId, authorId))
        {
            throw new ConflictException("You are already subscribed");
        }

        var sub = new Subscription
        {
            AuthorId = authorId,
            UserId = userId
        };

        await subscriptionRepository.SubscribeAsync(sub);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnsubscribeAsync(Guid authorId, Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true 
            : throw new NotFoundException("User not found");

        _ = await entityValidationService.IsUserExistsAsync(authorId) ? true 
            : throw new NotFoundException("Author not found");

        var sub = await subscriptionRepository.GetSubscriptionAsync(userId, authorId) ??
                  throw new ConflictException("You are not subscribed");

        subscriptionRepository.UnSubscribe(sub);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<bool> IsSubscribedAsync(Guid userId, Guid authorId)
    {
        return await subscriptionRepository.IsSubscribedAsync(userId, authorId);
    }

}