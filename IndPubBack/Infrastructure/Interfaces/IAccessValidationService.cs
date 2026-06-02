namespace IndPubBack.Infrastructure.Interfaces;

public interface IAccessValidationService
{
    Task EnsureUserIsAuthorAsync(Guid userId, Guid bookId);
    Task EnsureUserIsModeratorAsync(Guid userId);
    Task EnsureUserIsAuthorOrModeratorAsync(Guid userId, Guid bookId);
    Task EnsureUserIsReviewAuthorOrModeratorAsync(Guid userId, Guid reviewId);
    Task EnsureUserIsNotificationOwnerOrModeratorAsync(Guid userId, Guid notificationId);
}