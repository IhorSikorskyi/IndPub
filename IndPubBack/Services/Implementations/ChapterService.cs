using IndPubBack.DTOs.Requests.Chapter;
using IndPubBack.DTOs.Responses.Chapter;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class ChapterService(
    IChapterRepository chapterRepository,
    IUnitOfWork unitOfWork,
    IEntityValidationService entityValidationService,
    IAccessValidationService accessValidationService) : IChapterService
{
    private const string BookNotFoundMessage = "Book not found";
    private const string UserNotFoundMessage = "User not found";

    public async Task<ChapterResponse> CreateChapterAsync(Guid bookId, Guid authorId, ChapterCreateRequest request)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true :
            throw new NotFoundException(BookNotFoundMessage);
        _ = await entityValidationService.IsUserExistsAsync(authorId) ? true :
            throw new NotFoundException(UserNotFoundMessage);
        _ = await accessValidationService.IsUserIsAuthorAsync(authorId, bookId) ? true :
            throw new ForbiddenException("You are not an author of this book");


        var chapterNumber = await chapterRepository.GetNextChapterNumberAsync(bookId);

        var chapter = new Chapter
        {
            BookId = bookId,
            Title = string.IsNullOrWhiteSpace(request.Title) 
                ? $"Chapter {chapterNumber}" 
                : request.Title,
            Content = request.Content,
            ChapterNumber = chapterNumber
        };

        await chapterRepository.AddAsync(chapter);
        await unitOfWork.SaveChangesAsync();

        return MapToChapterResponse(chapter);
    }
    public async Task<ChapterResponse> UpdateChapterAsync(Guid bookId, Guid chapterId, Guid authorId, ChapterUpdateRequest request)
    {
        _ = await entityValidationService.IsUserExistsAsync(authorId) ? true :
            throw new NotFoundException(UserNotFoundMessage);
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true :
            throw new NotFoundException(BookNotFoundMessage);
        _ = await accessValidationService.IsUserIsAuthorAsync(authorId, bookId) ? true :
            throw new ForbiddenException("You are not an author of this book");

        var chapter = await chapterRepository.GetByIdAsync(chapterId) ?? throw new NotFoundException("Chapter not found");

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            chapter.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            chapter.Content = request.Content;
        }

        chapterRepository.Update(chapter);
        await unitOfWork.SaveChangesAsync();

        return MapToChapterResponse(chapter);
    }

    public async Task<ChapterResponse> GetChapterAsync(Guid bookId, Guid chapterId)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true :
            throw new NotFoundException(BookNotFoundMessage);

        var chapter = await chapterRepository.GetByIdAsync(chapterId) 
                      ?? throw new NotFoundException("Chapter not found");

        return MapToChapterResponse(chapter);
    }

    public async Task<bool> DeleteChapterAsync(Guid bookId, Guid chapterId, Guid authorId)
    {
        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true :
            throw new NotFoundException(BookNotFoundMessage);

        var isAuthor = await accessValidationService.IsUserIsAuthorAsync(authorId, bookId);
        var isModerator = await accessValidationService.IsUserIsModeratorAsync(authorId);

        if (!isAuthor && !isModerator)
        {
            throw new ForbiddenException("You are not authorized to delete this chapter");
        }

        var chapter = await chapterRepository.GetByIdAsync(chapterId)
                      ?? throw new NotFoundException("Chapter not found");

        if (chapter.BookId != bookId)
        {
            throw new ValidationException("Chapter does not belong to the specified book");
        }

        chapterRepository.Delete(chapter);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    #region Helpers

    private static ChapterResponse MapToChapterResponse(Chapter chapter)
    {
        return new ChapterResponse
        {
            BookId = chapter.BookId,
            ChapterId = chapter.Id,
            Title = chapter.Title,
            Content = chapter.Content,
            ChapterNumber = chapter.ChapterNumber
        };
    }

    #endregion
}