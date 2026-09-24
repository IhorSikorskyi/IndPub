using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(Guid bookId, Guid userId, ReviewRequest request);
    Task<ReviewResponse> UpdateReviewAsync(Guid reviewId, Guid bookId, Guid userId, ReviewRequest request);
    Task<ReviewResponse> GetReviewByIdAsync(Guid reviewId);
    Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId);

    Task<IEnumerable<ReviewResponse>> GetAllReviewsForBookAsync(Guid bookId);
    Task<ReviewResponse> GetUserReviewAsync(Guid bookId, Guid userId);
    Task<IEnumerable<ReviewResponse>> GetAllReviewsByUserAsync(Guid userId);
}