using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class LibraryRepository(Connected dbContext) : Repository<LibraryEntry>(dbContext), ILibraryRepository
{
    public async Task<LibraryEntry?> GetByIdAsync(Guid userId, Guid bookId)
    {
        return await dbContext.Libraries
            .Where(e => e.UserId == userId && e.BookId == bookId)
            .Include(e => e.Book)
            .FirstOrDefaultAsync();
    }

    public async Task<IList<LibraryEntry>> GetCursorPageAsync(Guid userId, LibraryBookStatus status, DateTime? cursor,
        int pageSize)
    {
        IQueryable<LibraryEntry> query = dbContext.Libraries
            .Where(e => e.UserId == userId && e.Status == status);

        if (cursor != null)
        {
            query = query.Where(e => e.DateAdded < cursor);
        }

        return await query
            .OrderByDescending(e => e.DateAdded)
            .Take(pageSize)
            .Include(e => e.Book)
            .ToListAsync();
    }

    public async Task<bool> DeleteFromLibraryAsync(Guid userId, Guid bookId)
    {
        var entity = await GetByIdAsync(userId, bookId) ??
                     throw new NotFoundException("Library entry not found");

        dbContext.Libraries.Remove(entity);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId)
    {
        return await dbContext.Libraries
            .AnyAsync(e => e.UserId == userId && e.BookId == bookId);
    }

    public async Task<bool> UpdateLibraryEntryStatusAsync(Guid userId, Guid bookId, LibraryBookStatus status)
    {
        var entity = await GetByIdAsync(userId, bookId) ??
                     throw new NotFoundException("Library entry not found");

        entity.Status = status;
        await dbContext.SaveChangesAsync();

        return true;
    }
}