using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class BookRepository(Connected dbContext) : Repository<Book>(dbContext), IBookRepository
{
    public async Task<Book?> GetByTitleAsync(string title)
    {
        return await dbContext.Books.FirstOrDefaultAsync(b => b.Title == title);
    }

    public async Task<bool> HasTitleAsync(string title)
    {
        return await dbContext.Books.AnyAsync(b => b.Title == title);
    }

    public async Task<List<Book>> GetBooksByAuthorIdAsync(List<Guid> authorId)
    {
        return await dbContext.Books
            .Where(b => b.BookAuthors
                .Any(ba => authorId.Contains(ba.UserId)))
            .ToListAsync();
    }

    public override async Task<Book?> GetByIdAsync(Guid id)
    {
        return await dbContext.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.User)
            .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
            .Include(b => b.Chapters)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

}