using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface ILibraryRepository: IRepository<LibraryEntry>
{
    Task<IList<LibraryEntry>> GetCursorPageAsync(Guid userId, LibraryBookStatus status, DateTime? cursor, int pageSize);
}