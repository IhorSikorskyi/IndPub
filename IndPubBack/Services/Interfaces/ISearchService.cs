using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISearchService
{
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByFiltersAsync(BookSearchRequest request);
}
