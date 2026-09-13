using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name);
    Task<Tag> AddAsync(string name);
}