using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names);
    Task<Tag> AddAsync(string name);
}