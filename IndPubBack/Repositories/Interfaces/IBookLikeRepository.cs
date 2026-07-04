using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookLikeRepository : IRepository<BookLike>
{
    Task<BookLike?> GetLikedAsync(Guid bookId, Guid userId);
    Task<IList<BookLike>> GetBookLikesListAsync(Guid userId, DateTime? cursor, int pageSize);
    Task UnLikeBookAsync(BookLike like);
    Task<bool> IsBookLikedAsync(Guid bookId, Guid userId);
}