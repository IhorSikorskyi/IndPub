using IndPubBack.DTOs.Requests.Library;
using IndPubBack.DTOs.Responses.User;

namespace IndPubBack.Services.Interfaces;

public interface ILibraryService
{
    Task<UserActivitiesResponse> GetLibraryAsync(Guid userId, LibraryListRequest request);
    Task<bool> AddToLibraryAsync(Guid bookId, Guid userId);
    Task<bool> RemoveFromLibraryAsync(Guid bookId, Guid userId);
    Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId);
    Task<bool> UpdateLibraryEntryStatusAsync(Guid userId, LibraryEntryRequest request);
}