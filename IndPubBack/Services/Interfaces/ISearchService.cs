using IndPubBack.DTOs.Requests.Search;
using IndPubBack.DTOs.Responses.Book;

namespace IndPubBack.Services.Interfaces;

public interface ISearchService
{
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByFiltersAsync(BookSearchRequest request);
}
