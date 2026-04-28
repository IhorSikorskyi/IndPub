using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByTitleAsync(string title);
}