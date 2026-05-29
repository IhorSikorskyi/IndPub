using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class AccessValidationService(IUserRepository userRepository, IBookRepository bookRepository) : IAccessValidationService
{
    public async Task EnsureUserIsAuthorAsync(Guid userId, Guid bookId)
    {
        var isAuthor = await bookRepository.IsUserAuthorAsync(userId, bookId);
        if (!isAuthor)
        {
            throw new ForbiddenException("You are not an author of this book");
        }
    }

    public async Task EnsureUserIsModeratorAsync(Guid userId)
    {
        var role = await userRepository.GetUserRoleAsync(userId);
        if (role != "Moderator")
        {
            throw new ForbiddenException("You are not a moderator");
        }
    }

    public async Task EnsureUserIsAuthorOrModeratorAsync(Guid userId, Guid bookId)
    {
        var isAuthor = await bookRepository.IsUserAuthorAsync(userId, bookId);
        var role = await userRepository.GetUserRoleAsync(userId);

        if(!isAuthor && role != nameof(Roles.Moderator))
        {
            throw new ForbiddenException("You are not an author or a moderator of this book");
        }
    }
}