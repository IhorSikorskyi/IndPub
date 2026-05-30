using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IReviewLikeRepository : IRepository<ReviewLike>
{
    Task<bool> LikeInteractionAsync(Guid reviewId, Guid userId);
}