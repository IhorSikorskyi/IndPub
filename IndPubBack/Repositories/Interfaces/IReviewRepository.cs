using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetAllReviewsForBookAsync(Guid bookId);
    Task<IEnumerable<Review>> GetAllReviewsByUserAsync(Guid userId);
    Task<Review> GetUserReviewAsync(Guid bookId, Guid userId);
}