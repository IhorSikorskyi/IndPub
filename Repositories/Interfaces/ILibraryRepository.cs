using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface ILibraryRepository: IRepository<LibraryEntry>
{
    Task<LibraryEntry?> GetByIdAsync(Guid userId, Guid bookId);
    Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId);
    Task<IList<LibraryEntry>> GetCursorPageAsync(Guid userId, LibraryBookStatus status, DateTime? cursor, int pageSize);
    Task<bool> DeleteFromLibraryAsync(Guid userId, Guid bookId);
    Task<bool> UpdateLibraryEntryStatusAsync(Guid userId, Guid bookId, LibraryBookStatus status);
}