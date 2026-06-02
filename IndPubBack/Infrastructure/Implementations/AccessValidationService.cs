using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class AccessValidationService(
    IUserRepository userRepository, 
    IBookRepository bookRepository, 
    IReviewRepository reviewRepository,
    INotificationRepository notificationRepository
    ) : IAccessValidationService
{
    public async Task EnsureUserIsAuthorAsync(Guid userId, Guid bookId)
    {
        var isAuthor = await bookRepository.IsUserAuthorAsync(userId, bookId);
        if (!isAuthor)
        {
            throw new ForbiddenException("You are not an author of this book");
        }
    }

    public async Task EnsureUserIsModeratorAsync(Guid userId)
    {
        var role = await userRepository.GetUserRoleAsync(userId);
        if (role != "Moderator")
        {
            throw new ForbiddenException("You are not a moderator");
        }
    }

    public async Task EnsureUserIsAuthorOrModeratorAsync(Guid userId, Guid bookId)
    {
        var isAuthor = await bookRepository.IsUserAuthorAsync(userId, bookId);
        var role = await userRepository.GetUserRoleAsync(userId);

        if(!isAuthor && role != nameof(Roles.Moderator))
        {
            throw new ForbiddenException("You are not an author or a moderator of this book");
        }
    }

    public async Task EnsureUserIsReviewAuthorOrModeratorAsync(Guid userId, Guid reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId)
                     ?? throw new NotFoundException("Review not found");

        var role = await userRepository.GetUserRoleAsync(userId);

        if (review.UserId != userId && role != nameof(Roles.Moderator))
        {
            throw new ForbiddenException("You are not the author of this review.");
        }
    }

    public async Task EnsureUserIsNotificationOwnerOrModeratorAsync(Guid userId, Guid notificationId)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId)
                           ?? throw new NotFoundException("Notification not found");

        var role = await userRepository.GetUserRoleAsync(userId);

        if (notification.UserId != userId && role != nameof(Roles.Moderator))
        {
            throw new ForbiddenException("You are not the owner of this notification.");
        }
    }
}