using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(Guid bookId, Guid userId, ReviewRequest request);
    Task<ReviewResponse> UpdateReviewAsync(Guid reviewId, Guid bookId, Guid userId, ReviewRequest request);
    Task<ReviewResponse> GetReviewByIdAsync(Guid reviewId);
    Task DeleteReviewAsync(Guid userId, Guid reviewId);

    Task<IEnumerable<ReviewResponse>> GetAllReviewsForBookAsync(Guid bookId);
    Task<ReviewResponse> GetUserReviewAsync(Guid bookId, Guid userId);
    Task<bool> LikeReviewInteractionAsync(Guid reviewId, Guid userId);
}