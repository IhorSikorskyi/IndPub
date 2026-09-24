using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class AccessValidationService(
    IUserRepository userRepository,
    IBookRepository bookRepository
    ) : IAccessValidationService
{
    public async Task<bool> IsUserIsAuthorAsync(Guid userId, Guid bookId)
    {
        var isAuthor = await bookRepository.IsUserAuthorAsync(userId, bookId);

        return isAuthor;
    }

    public async Task<bool> IsUserIsModeratorAsync(Guid userId)
    {
        var role = await userRepository.GetUserRoleAsync(userId);
        if (role != "Moderator")
        {
            return false;
        }

        return true;
    }
}