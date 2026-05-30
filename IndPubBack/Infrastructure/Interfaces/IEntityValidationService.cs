namespace IndPubBack.Infrastructure.Interfaces;

public interface IEntityValidationService
{
    Task EnsureUserExistsAsync(Guid userId);
    Task<bool> IsUserExistsAsync(string loginOrEmail);
    Task EnsureBookExistsAsync(Guid bookId);
    Task EnsureChapterExistsAsync(Guid chapterId);
    Task EnsureChapterBelongToBookAsync(Guid bookId, Guid chapterId);
    Task EnsureReviewExistsAsync(Guid reviewId);
    Task EnsureReviewBelongToBookAsync(Guid reviewId, Guid bookId);
}