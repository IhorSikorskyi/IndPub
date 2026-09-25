using IndPubBack.DTOs.Requests.Chapter;
using IndPubBack.DTOs.Responses.Chapter;

namespace IndPubBack.Services.Interfaces;

public interface IChapterService
{
    Task<ChapterResponse> CreateChapterAsync(Guid bookId, Guid authorId, ChapterCreateRequest request);
    Task<ChapterResponse> UpdateChapterAsync(Guid bookId, Guid chapterId, Guid authorId, ChapterUpdateRequest request);
    Task<ChapterResponse> GetChapterAsync(Guid bookId, Guid chapterId);
    Task<bool> DeleteChapterAsync(Guid bookId, Guid chapterId, Guid authorId);
}