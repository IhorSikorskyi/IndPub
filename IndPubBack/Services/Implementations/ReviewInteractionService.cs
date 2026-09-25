using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Implementations;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class ReviewInteractionService(
    IReviewLikeRepository reviewLikeRepository,
    IUnitOfWork unitOfWork,
    IEntityValidationService entityValidationService) : IReviewInteractionService
{
    public async Task<bool> LikeReviewAsync(Guid reviewId, Guid userId)
    {
        _ = await entityValidationService.IsReviewExistsAsync(reviewId) ? true
            : throw new NotFoundException("Review not found");

        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        if (await IsReviewLiked(reviewId, userId))
        {
            throw new ConflictException("Review is liked");
        }

        var like = new ReviewLike
        {
            ReviewId = reviewId,
            UserId = userId
        };

        await reviewLikeRepository.AddAsync(like);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnLikeReviewAsync(Guid reviewId, Guid userId)
    {
        _ = await entityValidationService.IsReviewExistsAsync(reviewId) ? true
            : throw new NotFoundException("Review not found");

        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        var like = await reviewLikeRepository.GetByIdAsync(reviewId, userId)
                   ?? throw new ConflictException("Review is not liked");

        await reviewLikeRepository.UnLikeReviewAsync(like);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<bool> IsReviewLiked(Guid reviewId, Guid userId)
    {
        return await reviewLikeRepository.IsReviewLikedAsync(reviewId, userId);
    }
}