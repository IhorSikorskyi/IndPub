using IndPubBack.Entities;
using IndPubBack.Enums;

namespace IndPubBack.Repositories.Interfaces;

public interface ILibraryRepository
{
    Task<LibraryEntry?> GetByIdAsync(Guid userId, Guid bookId);
    Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId);
    Task<IList<LibraryEntry>> GetCursorPageAsync(Guid userId, LibraryBookStatus status, DateTime? cursor, int pageSize);

    Task AddAsync(LibraryEntry libraryEntry);
    void DeleteFromLibrary(LibraryEntry libraryEntry);
    void UpdateLibraryEntryStatus(LibraryEntry libraryEntry, LibraryBookStatus status);
}