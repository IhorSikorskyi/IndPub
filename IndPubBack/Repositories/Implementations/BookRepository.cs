using IndPubBack.DTO.Requests;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class BookRepository(Connected dbContext) : Repository<Book>(dbContext), IBookRepository
{
    public override async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await dbContext.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.User)
            .Include(b => b.Genre)
            .Include(b => b.Reviews)
            .Include(b => b.Chapters)
            .ToListAsync();

    }

    public async Task<bool> HasTitleAsync(string title)
    {
        return await dbContext.Books.AnyAsync(b => b.Title == title);
    }

    public override async Task<Book?> GetByIdAsync(Guid id)
    {
        return await dbContext.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.User)
            .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
            .Include(b => b.Chapters)
            .Include(b => b.Genre)
            .Include(b => b.BookLikes)
            .Include(b => b.Reviews)
                .ThenInclude(r => r.ReviewLikes)
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .Include(b => b.Category)
            .Include(b => b.Subcategory)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IList<Book>> SearchAsync(BookSearchRequest request)
    {
        var query = dbContext.Books.AsQueryable();
        query = ApplyFilters(query, request);
        query = ApplySorting(query, request);
        query = ApplyCursor(query, request.Cursor);

        return await query.Take(request.PageSize).ToListAsync();
    }

    private static IQueryable<Book> ApplyFilters(IQueryable<Book> query, BookSearchRequest request)
    {
        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(b => b.Title.Contains(request.Title));
        }

        if (request.AuthorName?.Count > 0)
        {
            query = query.Where(b => b.BookAuthors.Any(ba => request.AuthorName.Contains(ba.User.Login)));
        }

        if (request.PublishDateFrom.HasValue)
        {
            query = query.Where(b => b.PublishedDate >= request.PublishDateFrom.Value);
        }

        if (request.PublishDateTo.HasValue)
        {
            query = query.Where(b => b.PublishedDate <= request.PublishDateTo.Value);
        }

        if (request.UpdatedDateFrom.HasValue)
        {
            query = query.Where(b => b.UpdatedDate >= request.UpdatedDateFrom.Value);
        }

        if (request.UpdatedDateTo.HasValue)
        {
            query = query.Where(b => b.UpdatedDate <= request.UpdatedDateTo.Value);
        }

        if (request.Language.HasValue)
        {
            query = query.Where(b => b.Language == request.Language.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        if (!string.IsNullOrEmpty(request.GenreName))
        {
            query = query.Where(b => b.Genre.Name == request.GenreName);
        }

        if (!string.IsNullOrEmpty(request.CategoryName))
        {
            query = query.Where(b => b.Category.Name == request.CategoryName);
        }

        if (!string.IsNullOrEmpty(request.SubcategoryName))
        {
            query = query.Where(b => b.Subcategory.Name == request.SubcategoryName);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(b => b.Rating >= request.MinRating.Value);
        }

        if (request.MinLikes.HasValue)
        {
            query = query.Where(b => b.BookLikes.Count >= request.MinLikes.Value);
        }

        if (request.MinChapters.HasValue)
        {
            query = query.Where(b => b.Chapters.Count >= request.MinChapters.Value);
        }

        if (request.BookTagName?.Count > 0)
        {
            query = query.Where(b => b.BookTags.Any(bt => request.BookTagName.Contains(bt.Tag.Name)));
        }

        return query;
    }

    private static IQueryable<Book> ApplySorting(IQueryable<Book> query, BookSearchRequest request)
    {
        return request.SortingBy switch
        {
            BookSortingBy.Title => request.Descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            BookSortingBy.PublishDate => request.Descending ? query.OrderByDescending(b => b.PublishedDate) : query.OrderBy(b => b.PublishedDate),
            BookSortingBy.UpdatedDate => request.Descending ? query.OrderByDescending(b => b.UpdatedDate) : query.OrderBy(b => b.UpdatedDate),
            BookSortingBy.LikesNumber => request.Descending ? query.OrderByDescending(b => b.BookLikes.Count) : query.OrderBy(b => b.BookLikes.Count),
            BookSortingBy.ChaptersNumber => request.Descending ? query.OrderByDescending(b => b.Chapters.Count) : query.OrderBy(b => b.Chapters.Count),
            BookSortingBy.Rating => request.Descending ? query.OrderByDescending(b => b.Rating) : query.OrderBy(b => b.Rating),
            _ => query
        };
    }

    private static IQueryable<Book> ApplyCursor(IQueryable<Book> query, string? cursor)
    {
        if (string.IsNullOrEmpty(cursor) || !Guid.TryParse(cursor, out var cursorId))
        {
            return query;
        }

        return query.Where(b => b.Id != cursorId);
    }
}