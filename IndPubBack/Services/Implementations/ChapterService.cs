using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class ChapterService(
    IChapterRepository chapterRepository, 
    IEntityValidationService entityValidationService,
    IAccessValidationService accessValidationService) : IChapterService
{
    public async Task<ChapterResponse> CreateChapterAsync(Guid bookId, Guid authorId, ChapterCreateRequest request)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);
        await entityValidationService.EnsureUserExistsAsync(authorId);
        await accessValidationService.EnsureUserIsAuthorAsync(authorId, bookId);

        var chapterNumber = await chapterRepository.GetNextChapterNumberAsync(bookId);

        var chapter = new Chapter()
        {
            BookId = bookId,
            Title = string.IsNullOrWhiteSpace(request.Title)
                ? $"Chapter {chapterNumber}"
                : request.Title,
            Content = request.Content,
            ChapterNumber = chapterNumber
        };

        await chapterRepository.AddAsync(chapter);

        return MapToChapterResponse(chapter);
    }
    public async Task<ChapterResponse> UpdateChapterAsync(Guid bookId, Guid chapterId, Guid authorId, ChapterUpdateRequest request)
    {
        await entityValidationService.EnsureUserExistsAsync(authorId);
        await entityValidationService.EnsureBookExistsAsync(bookId);
        await accessValidationService.EnsureUserIsAuthorAsync(authorId, bookId);

        var chapter = await chapterRepository.GetByIdAsync(chapterId) ?? throw new NotFoundException("Chapter not found");

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            chapter.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            chapter.Content = request.Content;
        }

        await chapterRepository.UpdateAsync(chapter);

        return MapToChapterResponse(chapter);
    }

    public async Task<ChapterResponse> GetChapterAsync(Guid bookId, Guid chapterId)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);

        var chapter = await chapterRepository.GetByIdAsync(chapterId) ?? throw new NotFoundException("Chapter not found");
        
        return MapToChapterResponse(chapter);
    }

    public async Task<bool> DeleteChapterAsync(Guid bookId, Guid chapterId, Guid authorId)
    {
        await entityValidationService.EnsureBookExistsAsync(bookId);
        await entityValidationService.EnsureChapterExistsAsync(chapterId);
        await entityValidationService.EnsureChapterBelongToBookAsync(bookId, chapterId);
        await accessValidationService.EnsureUserIsAuthorOrModeratorAsync(authorId, bookId);

        await chapterRepository.DeleteAsync(chapterId);

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