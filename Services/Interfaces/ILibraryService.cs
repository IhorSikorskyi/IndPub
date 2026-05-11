using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ILibraryService
{
    // Бібліотека
    Task<IList<BookResponse>> GetLibraryAsync(Guid userId);
    Task<bool> AddToLibraryAsync(Guid bookId, Guid userId);
    Task<bool> RemoveFromLibraryAsync(Guid bookId, Guid userId);
}