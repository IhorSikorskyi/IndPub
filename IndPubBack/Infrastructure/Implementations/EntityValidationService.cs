using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class EntityValidationService(
    IUserRepository userRepository,
    IBookRepository bookRepository,
    IChapterRepository chapterRepository) : IEntityValidationService
{
    public async Task EnsureUserExistsAsync(Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }
    }
    
    public async Task<bool> IsUserExistsAsync(string loginOrEmail)
    {
        return await userRepository.IsExistByLoginOrEmailAsync(loginOrEmail);
    }

    public async Task EnsureBookExistsAsync(Guid bookId)
    {
        if (!await bookRepository.IsExistAsync(bookId))
        {
            throw new NotFoundException("Book not found");
        }
    }

    public async Task EnsureChapterExistsAsync(Guid chapterId)
    {
        if (!await chapterRepository.IsExistAsync(chapterId))
        {
            throw new NotFoundException("Chapter not found");
        }
    }

    public async Task EnsureChapterBelongToBookAsync(Guid bookId, Guid chapterId)
    {
        var chapter = await chapterRepository.GetByIdAsync(chapterId) ?? throw new NotFoundException("Chapter not found");
        if (chapter.BookId != bookId)
        {
            throw new NotFoundException("Chapter does not belong to the specified book");
        }
    }
}