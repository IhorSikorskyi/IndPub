using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IBookService
{
    Task<BookResponse> CreateBookAsync(BookCreateRequest request);
    Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid bookId, Guid userId);
    Task<bool> DeleteBookAsync(Guid bookId, Guid userId);
    Task<BookResponse> GetBookByIdAsync(Guid bookId);
}