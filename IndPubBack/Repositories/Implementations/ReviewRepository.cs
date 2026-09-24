using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ReviewRepository(IndPubDbContext dbContext) : Repository<Review>(dbContext), IReviewRepository
{
    public async Task<IEnumerable<Review>> GetAllReviewsForBookAsync(Guid bookId)
    {
        return await dbContext.Reviews
            .Where(r => r.BookId == bookId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByUserAsync(Guid userId)
    {
        return await dbContext.Reviews
            .Where(r => r.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetUserReviewAsync(Guid bookId, Guid userId)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
    }
}