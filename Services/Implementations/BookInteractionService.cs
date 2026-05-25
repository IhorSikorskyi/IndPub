using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookInteractionService(IEntityValidationService entityValidationService, IBookLikeRepository bookLikeRepository) : IBookInteractionService
{
    public async Task<bool> LikeBookAsync(Guid bookId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);
        await entityValidationService.EnsureBookExistsAsync(bookId);

        if (await bookLikeRepository.IsLikedAsync(bookId, userId))
        {
            throw new ConflictException("Book is already liked.");
        }

        await bookLikeRepository.AddAsync(new BookLike
        {
            BookId = bookId,
            UserId = userId
        });

        return true;
    }

    public async Task<bool> UnlikeBookAsync(Guid bookId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        await entityValidationService.EnsureBookExistsAsync(bookId);

        if (!await bookLikeRepository.IsLikedAsync(bookId, userId))
        {
            throw new ConflictException("Book is not liked.");
        }

        await bookLikeRepository.UnlikeBookAsync(bookId, userId);
        
        return true;
    }
}