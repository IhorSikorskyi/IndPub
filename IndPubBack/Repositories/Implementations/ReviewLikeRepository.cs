using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ReviewLikeRepository(Connected dbContext) : Repository<ReviewLike>(dbContext), IReviewLikeRepository
{
    public async Task<IList<ReviewLike>> GetLikesAsync(Guid userId, DateTime? cursor, int pageSize)
    {
        IQueryable<ReviewLike> query = dbContext.ReviewLikes
            .Where(rl => rl.UserId == userId)
            .Include(rl => rl.Review);

        if (cursor != null)
        {
            query = query.Where(rl => rl.Review.CreatedAt < cursor);
        }

        return await query
            .OrderByDescending(rl => rl.LikedAt)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task UnLikeReviewAsync(ReviewLike like)
    {
        dbContext.ReviewLikes.Remove(like);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsReviewLikedAsync(Guid reviewId, Guid userId)
    {
        return await dbContext.ReviewLikes
            .AnyAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);
    }
}