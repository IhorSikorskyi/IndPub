namespace IndPubBack.Infrastructure.Interfaces;

public interface IEntityValidationService
{
    Task<bool> IsUserExistsAsync(Guid userId);
    Task<bool> IsBookExistsAsync(Guid bookId);
    Task<bool> IsReviewExistsAsync(Guid reviewId);
}