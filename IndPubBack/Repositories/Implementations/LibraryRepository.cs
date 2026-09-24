using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Enums;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class LibraryRepository(IndPubDbContext dbContext) : ILibraryRepository
{
    public async Task<LibraryEntry?> GetByIdAsync(Guid userId, Guid bookId)
    {
        return await dbContext.Libraries
            .Where(e => e.UserId == userId && e.BookId == bookId)
            .Include(e => e.Book)
            .AsNoTracking()
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
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId)
    {
        return await dbContext.Libraries
            .AnyAsync(e => e.UserId == userId && e.BookId == bookId);
    }

    public async Task AddAsync(LibraryEntry libraryEntry)
    {
        await dbContext.Libraries.AddAsync(libraryEntry);
    }

    public void DeleteFromLibrary(LibraryEntry libraryEntry)
    {
        dbContext.Libraries.Remove(libraryEntry);
    }

    public void UpdateLibraryEntryStatus(LibraryEntry libraryEntry, LibraryBookStatus status)
    {
        libraryEntry.Status = status;
    }
}