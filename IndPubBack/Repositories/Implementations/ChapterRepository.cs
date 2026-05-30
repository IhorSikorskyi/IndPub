using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class ChapterRepository(Connected dbContext) : Repository<Chapter>(dbContext), IChapterRepository
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

    public override async Task UpdateAsync(Chapter entity)
    {
        dbContext.Chapters.Update(entity);

        var book = await dbContext.Books.FindAsync(entity.BookId) ?? throw new NotFoundException("Book not found");

        book.UpdatedDate = _utcNow;

        await dbContext.SaveChangesAsync();
    }

    public override async Task DeleteAsync(Guid id)
    {
        var chapter = await dbContext.Chapters.FindAsync(id) ?? throw new NotFoundException("Chapter not found");
        
        var book = await dbContext.Books.FindAsync(chapter.BookId) ?? throw new NotFoundException("Book not found");

        var chaptersToReorder = await dbContext.Chapters
            .Where(c => c.BookId == chapter.BookId && c.ChapterNumber > chapter.ChapterNumber)
            .OrderBy(c => c.ChapterNumber)
            .ToListAsync();

        foreach (var c in chaptersToReorder)
        {
            c.ChapterNumber--;
        }

        dbContext.Chapters.Remove(chapter);

        book.UpdatedDate = _utcNow;

        await dbContext.SaveChangesAsync();
    }
}