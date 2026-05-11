using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IBookService
{
    // CRUD
    Task<BookResponse> CreateBookAsync(BookCreateRequest request);
    Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid bookId, Guid userId);
    Task<bool> DeleteBookAsync(Guid bookId, Guid userId);

    // Отримання
    //TODO: Make Requests for pagination and filtering
    Task<BookResponse> GetBookByIdAsync(Guid bookId);
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByAuthorIdAsync(Guid authorId);
    Task<IList<BookShortResponse>> GetBooksByGenreAsync(Guid genreId);
    Task<IList<BookShortResponse>> GetBooksByTagsAsync(Guid tagId);
    Task<IList<BookShortResponse>> SearchBooksAsync(string query);

    // Взаємодія
    Task<bool> LikeBookAsync(Guid bookId, Guid userId);
    Task<bool> UnlikeBookAsync(Guid bookId, Guid userId);
}