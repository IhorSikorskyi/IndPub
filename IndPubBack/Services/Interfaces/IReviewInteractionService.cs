namespace IndPubBack.Services.Interfaces;

public interface IReviewInteractionService
{
    Task<bool> LikeReviewAsync(Guid reviewId, Guid userId);
    Task<bool> UnLikeReviewAsync(Guid reviewId, Guid userId);
}