using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class SearchService(IBookRepository bookRepository) : ISearchService
{
    public async Task<IList<BookShortResponse>> GetAllBooksAsync()
    {
        var books = await bookRepository.GetAllAsync();
        return books.Select(MapToBookShortResponse).ToList();
    }

    public async Task<IList<BookShortResponse>> GetBooksByFiltersAsync(BookSearchRequest request)
    {
        var books = await bookRepository.SearchAsync(request);
        return books.Select(MapToBookShortResponse).ToList();
    }

    private static BookShortResponse MapToBookShortResponse(Book book)
    {
        return new BookShortResponse
        {
            BookId = book.Id,
            Title = book.Title,
            CoverImageUrl = book.CoverImageUrl,
            UpdatedDate = book.UpdatedDate,
            Language = book.Language,
            Status = book.Status,
            ChapterCount = book.Chapters.Count,
            GenreName = book.Genre.Name,
            Authors = [..book.BookAuthors.Select(ba => new AuthorResponse
            {
                Id = ba.User.Id,
                Login = ba.User.Login,
                ProfilePictureUrl = ba.User.ProfilePictureUrl
            })],
            Rating = book.Reviews.Count > 0 ? book.Reviews.Average(r => r.Rating) : 0
        };
    }
}