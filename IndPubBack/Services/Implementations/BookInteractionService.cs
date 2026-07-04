using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookInteractionService(
    IEntityValidationService entityValidationService,
    IBookLikeRepository bookLikeRepository) : IBookInteractionService
{
    public async Task<bool> LikeBookAsync(Guid bookId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);
        await entityValidationService.EnsureBookExistsAsync(bookId);

        if ( await IsBookLikedAsync(bookId, userId))
        {
            throw new ConflictException("Book liked");
        }

        var like = new BookLike
        {
            BookId = bookId,
            UserId = userId
        };

        await bookLikeRepository.AddAsync(like);

        return true;
    }

    public async Task<bool> UnLikeBookAsync(Guid bookId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);
        await entityValidationService.EnsureBookExistsAsync(bookId);

        var like = await bookLikeRepository.GetLikedAsync(bookId, userId) 
                   ?? throw new ConflictException("Book not liked");

        await bookLikeRepository.UnLikeBookAsync(like);
        return true;
    }

    private async Task<bool> IsBookLikedAsync(Guid bookId, Guid userId)
    {
        return await bookLikeRepository.IsBookLikedAsync(bookId, userId);
    }
}