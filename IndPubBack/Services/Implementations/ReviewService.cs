using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Entities;
using IndPubBack.Enums;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class ReviewService(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork,
    IEntityValidationService entityValidationService) : IReviewService
{
    private const string ReviewNotFoundMessage = "Review not found";

    #region CRUD

    public async Task<ReviewResponse> CreateReviewAsync(Guid bookId, Guid userId, ReviewRequest request)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true 
            : throw new NotFoundException("Book not found");
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true 
            : throw new NotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(request.ReviewText))
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
        await unitOfWork.SaveChangesAsync();

        return MapToReviewResponse(review);
    }

    public async Task<ReviewResponse> UpdateReviewAsync(Guid reviewId, Guid bookId, Guid userId, ReviewRequest request)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true
            : throw new NotFoundException("Book not found");
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        var review = await reviewRepository.GetByIdAsync(reviewId)
                     ?? throw new NotFoundException(ReviewNotFoundMessage);

        if (review.BookId != bookId)
        {
            throw new ValidationException("Review does not belong to the specified book.");
        }

        if (review.UserId != userId && review.User.Role != Roles.Moderator)
        {
            throw new ForbiddenException("You are not the author of this review.");
        }

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

        reviewRepository.Update(review);

        await unitOfWork.SaveChangesAsync();

        return MapToReviewResponse(review);
    }

    public async Task<ReviewResponse> GetReviewByIdAsync(Guid reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId)
                   ?? throw new NotFoundException("Review not found");

        return MapToReviewResponse(review);
    }

    public async Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId)
                     ?? throw new NotFoundException(ReviewNotFoundMessage);

        if (review.UserId != userId && review.User.Role != Roles.Moderator)
        {
            throw new ForbiddenException("You are not the author of this review.");
        }

        reviewRepository.Delete(review);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Interaction

    public async Task<IEnumerable<ReviewResponse>> GetAllReviewsForBookAsync(Guid bookId)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true
            : throw new NotFoundException("Book not found");
        var reviews = await reviewRepository.GetAllReviewsForBookAsync(bookId);

        return reviews.Select(MapToReviewResponse);
    }

    public async Task<ReviewResponse> GetUserReviewAsync(Guid bookId, Guid userId)
    {
        var review = await reviewRepository.GetUserReviewAsync(bookId, userId)
            ?? throw new NotFoundException(ReviewNotFoundMessage);
        return MapToReviewResponse(review);
    }

    public async Task<IEnumerable<ReviewResponse>> GetAllReviewsByUserAsync(Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        var reviews = await reviewRepository.GetAllReviewsByUserAsync(userId);
        return reviews.Select(MapToReviewResponse);
    }

    #endregion

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