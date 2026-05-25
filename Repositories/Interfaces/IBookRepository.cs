using IndPubBack.DTO.Requests;
using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<bool> HasTitleAsync(string title);

    Task<IList<Book>> SearchAsync(BookSearchRequest request);
}