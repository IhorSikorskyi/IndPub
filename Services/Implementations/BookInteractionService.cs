using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookInteractionService(IBookRepository bookRepository, IUserRepository userRepository) : IBookInteractionService
{
    #region Interaction

    //TODO: Add possibility to like only published books and prevent authors from liking their own books
    public async Task<bool> LikeBookAsync(Guid bookId, Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if (!await bookRepository.IsExistAsync(bookId))
        {
            throw new NotFoundException("Book not found");
        }

        throw new NotImplementedException();
    }

    //TODO: Add possibility to unlike only published books and prevent authors from unliking their own books
    public async Task<bool> UnlikeBookAsync(Guid bookId, Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if (!await bookRepository.IsExistAsync(bookId))
        {
            throw new NotFoundException("Book not found");
        }

        throw new NotImplementedException();
    }

    #endregion
}