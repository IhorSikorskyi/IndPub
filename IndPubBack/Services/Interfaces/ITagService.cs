using IndPubBack.DTOs.Requests;
using IndPubBack.Entities;

namespace IndPubBack.Services.Interfaces;

public interface ITagService
{
    Task<List<BookTag>> GetOrCreateBookTagsAsync(Guid bookId, List<CreateBookTagRequest> tagRequests);
}