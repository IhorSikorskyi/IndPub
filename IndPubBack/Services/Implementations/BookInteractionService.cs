using IndPubBack.Exceptions;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookInteractionService(
    IEntityValidationService entityValidationService,
    IUnitOfWork unitOfWork,
    IBookLikeRepository bookLikeRepository) : IBookInteractionService
{
    public async Task<bool> LikeBookAsync(Guid bookId, Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true : 
            throw new NotFoundException("User not found");

        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true : 
            throw new NotFoundException("Book not found");

        if ( await IsBookLikedAsync(bookId, userId))
        {
            throw new ConflictException("Book liked");
        }

        var like = new BookLike
        {
            BookId = bookId,
            UserId = userId
        };

        bookLikeRepository.LikeBook(like);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnLikeBookAsync(Guid bookId, Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true :
            throw new NotFoundException("User not found");

        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true :
            throw new NotFoundException("Book not found");

        var like = await bookLikeRepository.GetLikedAsync(bookId, userId) 
                   ?? throw new ConflictException("Book not liked");

        bookLikeRepository.UnLikeBook(like);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task<bool> IsBookLikedAsync(Guid bookId, Guid userId)
    {
        return await bookLikeRepository.IsBookLikedAsync(bookId, userId);
    }
}