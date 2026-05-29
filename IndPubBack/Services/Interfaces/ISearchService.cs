using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISearchService
{
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByFiltersAsync(BookSearchRequest request);
}
