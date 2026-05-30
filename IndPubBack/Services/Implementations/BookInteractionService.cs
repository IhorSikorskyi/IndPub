using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookInteractionService(IEntityValidationService entityValidationService, IBookLikeRepository bookLikeRepository) : IBookInteractionService
{
    public async Task<bool> LikeBookInteractionAsync(Guid bookId, Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);
        await entityValidationService.EnsureBookExistsAsync(bookId);
        
        return await bookLikeRepository.LikeInteractionAsync(bookId, userId);
    }
}