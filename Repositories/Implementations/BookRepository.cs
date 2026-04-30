using Microsoft.EntityFrameworkCore;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations;

public class BookRepository(Connected _context) : Repository<Book>(_context), IBookRepository
{
    public Task<Book?> GetByTitleAsync(string title)
    {
        return _context.Books.FirstOrDefaultAsync(b => b.Title == title);
    }

    public Task<bool> HasTitleAsync(string title)
    {
        return _context.Books.AnyAsync(b => b.Title == title);
    }
}