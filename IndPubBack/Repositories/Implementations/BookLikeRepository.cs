using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class BookLikeRepository(Connected dbContext) : Repository<BookLike>(dbContext), IBookLikeRepository
{
    public async Task<BookLike?> GetLikedAsync(Guid bookId, Guid userId)
    {
        return await dbContext.BookLikes
            .Where(bl => bl.UserId == userId && bl.BookId == bookId)
            .Include(bl => bl.Book)
            .FirstOrDefaultAsync();
    }

    public async Task<IList<BookLike>> GetBookLikesListAsync(Guid userId, DateTime? cursor, int pageSize)
    {
        IQueryable<BookLike> query = dbContext.BookLikes
            .Where(bl => bl.UserId == userId)
            .Include(bl => bl.Book);

        if (cursor != null)
        {
            query = query.Where(bl => bl.LikedAt < cursor);
        }

        return await query
            .OrderByDescending(bl => bl.LikedAt)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> IsLikedAsync(Guid bookId, Guid userId)
    {
        return await dbContext.BookLikes
            .AnyAsync(bl => bl.UserId == userId && bl.BookId == bookId);
    }

    public async Task<bool> UnlikeBookAsync(Guid bookId, Guid userId)
    {
        var entity = await GetLikedAsync(bookId, userId) ??
                     throw new NotFoundException("Book bot liked already.");

        dbContext.BookLikes.Remove(entity);
        await dbContext.SaveChangesAsync();

        return true;
    }
}