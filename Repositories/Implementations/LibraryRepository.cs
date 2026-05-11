using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class LibraryRepository(Connected dbContext) : Repository<LibraryEntry>(dbContext), ILibraryRepository
{
    public async Task<IList<LibraryEntry>> GetCursorPageAsync(Guid userId, LibraryBookStatus status, DateTime? cursor, int pageSize)
    {
        if (cursor is null)
        {
            return await dbContext.Libraries
                .Where(e => e.UserId == userId && e.Status == status)
                .OrderByDescending(e => e.DateAdded)
                .Take(pageSize)
                .Include(e => e.Book)
                .ToListAsync();
        }

        return await dbContext.Libraries
            .Where(e => e.UserId == userId)
            .Where(e => e.Status == status)
            .Where(e => e.DateAdded > cursor)
            .OrderByDescending(e => e.DateAdded)
            .Take(pageSize)
            .Include(e => e.Book)
            .ToListAsync();

    }
}