using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using System.Net;

namespace IndPubBack.Services.Implementations;

public class ReviewService(
    IReviewRepository reviewRepository, 
    IReviewLikeRepository reviewLikeRepository, 
    IEntityValidationService entityValidationService, 
    IAccessValidationService accessValidationService) : IReviewService
{
    #region CRUD

    public async Task<ReviewResponse> CreateReviewAsync(Guid bookId, Guid userId, ReviewRequest request)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);
        await entityValidationService.EnsureUserExistsAsync(userId);

        if(string.IsNullOrWhiteSpace(request.ReviewText))
        {
            throw new ValidationException("Review content cannot be empty.");
        }

        if (!request.Rating.HasValue || request.Rating < 0.1 || request.Rating > 5.0)
        {
            throw new ValidationException("Rating must be between 0.1 and 5.0.");
        }

        var review = new Review
        {
            Rating = request.Rating.Value,
            Text = request.ReviewText,
            BookId = bookId,
            UserId = userId
        };

        await reviewRepository.AddAsync(review);

        return MapToReviewResponse(review);
    }

    public async Task<ReviewResponse> UpdateReviewAsync(Guid reviewId, Guid bookId, Guid userId, ReviewRequest request)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);
        await entityValidationService.EnsureUserExistsAsync(userId);
        await entityValidationService.EnsureReviewExistsAsync(reviewId);
        await entityValidationService.EnsureReviewBelongToBookAsync(reviewId, bookId);
        await accessValidationService.EnsureUserIsReviewAuthorOrModeratorAsync(userId, reviewId);

        var review = await reviewRepository.GetByIdAsync(reviewId) 
                     ?? throw new NotFoundException("Review not found");

        if (!string.IsNullOrWhiteSpace(request.ReviewText))
        {
            review.Text = request.ReviewText;
        }

        if (request.Rating.HasValue)
        {
            if (request.Rating < 0.1 || request.Rating > 5.0)
            {
                throw new ValidationException("Rating must be between 0.1 and 5.0.");
            }
            review.Rating = request.Rating.Value;
        }

        await reviewRepository.UpdateAsync(review);

        return MapToReviewResponse(review);
    }

    public async Task<ReviewResponse> GetReviewByIdAsync(Guid reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId) 
                   ?? throw new NotFoundException("Review not found");

        return MapToReviewResponse(review);
    }

    public async Task DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        await entityValidationService.EnsureReviewExistsAsync(reviewId);
        await accessValidationService.EnsureUserIsReviewAuthorOrModeratorAsync(userId, reviewId);

        await reviewRepository.DeleteAsync(reviewId);
    }

    #endregion

    #region Interaction

    public async Task<IEnumerable<ReviewResponse>> GetAllReviewsForBookAsync(Guid bookId)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);
        var reviews = await reviewRepository.GetAllReviewsForBookAsync(bookId);

        return reviews.Select(MapToReviewResponse);
    }

    public async Task<ReviewResponse> GetUserReviewAsync(Guid bookId, Guid userId)
    {
        var review = await reviewRepository.GetUserReviewAsync(bookId, userId);
        return MapToReviewResponse(review);
    }

    public async Task<bool> LikeReviewAsync(Guid reviewId, Guid userId)
    {
        await entityValidationService.EnsureReviewExistsAsync(reviewId);
        await entityValidationService.EnsureUserExistsAsync(userId);

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

        return true;
    }

    public async Task<bool> UnLikeReviewAsync(Guid reviewId, Guid userId)
    {
        await entityValidationService.EnsureReviewExistsAsync(reviewId);
        await entityValidationService.EnsureUserExistsAsync(userId);

        var like = await reviewLikeRepository.GetByIdAsync(reviewId) 
                   ?? throw new ConflictException("Review is not liked");

        await reviewLikeRepository.UnLikeReviewAsync(like);

        return true;
    }

    #endregion

    private async Task<bool> IsReviewLiked(Guid reviewId, Guid userId)
    {
        return await reviewLikeRepository.IsReviewLikedAsync(reviewId, userId);
    }

    private static ReviewResponse MapToReviewResponse(Review review)
    {
        var response = new ReviewResponse
        {
            Id = review.Id,
            BookId = review.BookId,
            BookTitle = review.Book.Title,
            UserId = review.UserId,
            UserName = review.User.Login,
            Rating = review.Rating,
            Text = review.Text,
            CreatedAt = review.CreatedAt
        };
        return response;
    }
}