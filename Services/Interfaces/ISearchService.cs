using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface ISearchService
{
    // Отримання
    //TODO: Make Requests for pagination and filtering
    //TODO: Move to SearchService, StatisticsService, BookInteractionService
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByAuthorIdAsync(Guid authorId);
    Task<IList<BookShortResponse>> GetBooksByGenreAsync(Guid genreId);
    Task<IList<BookShortResponse>> GetBooksByTagsAsync(Guid tagId);
    Task<IList<BookShortResponse>> SearchBooksAsync(string query);
}