using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IChapterRepository : IRepository<Chapter>
{
    Task<int> GetNextChapterNumberAsync(Guid bookId);
}