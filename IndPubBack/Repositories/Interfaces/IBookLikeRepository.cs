using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookLikeRepository : IRepository<BookLike>
{
    Task<BookLike?> GetLikedAsync(Guid bookId, Guid userId);
    Task<IList<BookLike>> GetBookLikesListAsync(Guid userId, DateTime? cursor, int pageSize);
    Task<bool> IsLikedAsync(Guid bookId, Guid userId);
    Task<bool> UnlikeBookAsync(Guid bookId, Guid userId);
}