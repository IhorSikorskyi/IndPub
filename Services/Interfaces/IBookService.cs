using IndPubBack.DTO.Responses;
using IndPubBack.DTO.Requests;

namespace IndPubBack.Services.Interfaces;

public interface IBookService
{
    Task<BookCreateResponse> CreateBookAsync(BookCreateRequest request);
    Task<BookUpdateResponse> UpdateBookAsync(BookUpdateRequest request, Guid userId);
    Task<BookDeleteResponse> DeleteBookAsync(BookDeleteRequest request, Guid userId);
}