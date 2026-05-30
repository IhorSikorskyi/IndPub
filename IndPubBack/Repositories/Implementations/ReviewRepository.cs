using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ReviewRepository(Connected dbContext) : Repository<Review>(dbContext), IReviewRepository
{
    public async Task<IEnumerable<Review>> GetAllReviewsForBookAsync(Guid bookId)
    {
        var reviews = await dbContext.Reviews
            .Where(r => r.BookId == bookId)
            .ToListAsync();

        return reviews;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByUserAsync(Guid userId)
    {
        var reviews = await dbContext.Reviews
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return reviews;
    }

    public async Task<Review> GetUserReviewAsync(Guid bookId, Guid userId)
    {
        var review = await dbContext.Reviews
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId)
            ?? throw new NotFoundException("Review not found");

        return review;
    }
}