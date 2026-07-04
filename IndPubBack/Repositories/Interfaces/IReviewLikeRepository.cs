using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IReviewLikeRepository : IRepository<ReviewLike>
{
    Task<IList<ReviewLike>> GetLikesAsync(Guid userId, DateTime? cursor, int pageSize);
    Task UnLikeReviewAsync(ReviewLike like);
    Task<bool> IsReviewLikedAsync(Guid reviewId, Guid userId);
}