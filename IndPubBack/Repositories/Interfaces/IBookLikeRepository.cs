using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookLikeRepository
{
    Task<BookLike?> GetLikedAsync(Guid bookId, Guid userId);
    Task<IList<BookLike>> GetBookLikesListAsync(Guid userId, DateTime? cursor, int pageSize);
    void LikeBook(BookLike like);
    void UnLikeBook(BookLike like);
    Task<bool> IsBookLikedAsync(Guid bookId, Guid userId);
}