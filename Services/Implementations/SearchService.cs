using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class SearchService : ISearchService
{
    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetAllBooksAsync()
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByAuthorIdAsync(Guid authorId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByGenreAsync(Guid genreId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByTagsAsync(Guid tagId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> SearchBooksAsync(string query)
    {
        throw new NotImplementedException();
    }
}