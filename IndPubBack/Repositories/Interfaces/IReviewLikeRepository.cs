using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface IReviewLikeRepository
{
    Task AddAsync(ReviewLike like);
    Task<ReviewLike?> GetByIdAsync(Guid reviewId, Guid userId);
    Task<IList<ReviewLike>> GetLikesAsync(Guid userId, DateTime? cursor, int pageSize);
    Task UnLikeReviewAsync(ReviewLike like);
    Task<bool> IsReviewLikedAsync(Guid reviewId, Guid userId);
}