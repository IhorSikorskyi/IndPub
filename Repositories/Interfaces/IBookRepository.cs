using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByTitleAsync(string title);
    Task<bool> HasTitleAsync(string title);
    Task<List<Book>> GetBooksByAuthorIdAsync(List<Guid> authorId);
}