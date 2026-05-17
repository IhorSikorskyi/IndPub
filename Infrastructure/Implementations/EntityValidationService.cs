using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class EntityValidationService(
    IUserRepository userRepository,
    IBookRepository bookRepository) : IEntityValidationService
{
    public async Task EnsureUserExistsAsync(Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
            throw new NotFoundException("User not found");
    }

    public async Task EnsureBookExistsAsync(Guid bookId)
    {
        if (!await bookRepository.IsExistAsync(bookId))
            throw new NotFoundException("Book not found");
    }
}