using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IChapterService
{
    Task<ChapterResponse> CreateChapterAsync(Guid bookId, Guid authorId, ChapterCreateRequest request);
    Task<ChapterResponse> UpdateChapterAsync(Guid bookId, Guid chapterId, Guid authorId, ChapterUpdateRequest request);
    Task<ChapterResponse> GetChapterAsync(Guid bookId, Guid chapterId);
    Task<bool> DeleteChapterAsync(Guid bookId, Guid chapterId, Guid authorId);
}