using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ChapterRepository(IndPubDbContext dbContext) : Repository<Chapter>(dbContext), IChapterRepository
{
    private readonly DateTime _utcNow = DateTime.UtcNow;
    public async Task<int> GetNextChapterNumberAsync(Guid bookId)
    {
        var lastChapter = await dbContext.Chapters
            .Where(c => c.BookId == bookId)
            .OrderByDescending(c => c.ChapterNumber)
            .FirstOrDefaultAsync();
        return lastChapter is null ? 1 : lastChapter.ChapterNumber + 1;
    }

    public override async Task AddAsync(Chapter entity)
    {
        await dbContext.Chapters.AddAsync(entity);

        var book = await dbContext.Books.FindAsync(entity.BookId) ?? throw new NotFoundException("Book not found");

        book.UpdatedDate = _utcNow;

        await dbContext.SaveChangesAsync();
    }
}