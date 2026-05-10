using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IBookService
{
    // CRUD
    Task<BookResponse> CreateBookAsync(BookCreateRequest request);
    Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid userId);
    Task<DeleteResponse> DeleteBookAsync(BookDeleteRequest request, Guid userId);

    // Отримання
    Task<BookResponse> GetBookByIdAsync(Guid bookId);
    Task<IList<BookShortResponse>> GetAllBooksAsync();
    Task<IList<BookShortResponse>> GetBooksByAuthorIdAsync(Guid authorId);
    Task<IList<BookShortResponse>> GetBooksByGenreAsync(Guid genreId);
    Task<IList<BookShortResponse>> GetBooksByTagsAsync(Guid genreId);
    Task<IList<BookShortResponse>> SearchBooksAsync(string query);

    // Взаємодія
    Task LikeBookAsync(Guid bookId, Guid userId);
    Task UnlikeBookAsync(Guid bookId, Guid userId);

    // Бібліотека користувача
    Task AddToLibraryAsync(Guid bookId, Guid userId);
    Task RemoveFromLibraryAsync(Guid bookId, Guid userId);
}