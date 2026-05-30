using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ReviewLikeRepository(Connected dbContext) : Repository<ReviewLike>(dbContext), IReviewLikeRepository
{
    public async Task<bool> LikeInteractionAsync(Guid reviewId, Guid userId)
    {
        var like = await dbContext.ReviewLikes
            .FirstOrDefaultAsync(l => l.ReviewId == reviewId && l.UserId == userId);

        if (like is null)
        {
            like = new ReviewLike
            {
                ReviewId = reviewId,
                UserId = userId
            };
            await dbContext.ReviewLikes.AddAsync(like);
            await dbContext.SaveChangesAsync();

            return true;
        }

        dbContext.ReviewLikes.Remove(like);
        await dbContext.SaveChangesAsync();
        return false;
    }
}