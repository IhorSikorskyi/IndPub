using IndPubBack.DTOs.Requests.Book;
using IndPubBack.DTOs.Responses.Book;

namespace IndPubBack.Services.Interfaces;

public interface IBookService
{
    Task<BookResponse> CreateBookAsync(BookCreateRequest request, Guid currentUserId);
    Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid bookId, Guid userId);
    Task<bool> DeleteBookAsync(Guid bookId, Guid userId);
    Task<BookResponse> GetBookByIdAsync(Guid bookId);
}