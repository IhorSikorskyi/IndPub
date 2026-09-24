using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class TagRepository(IndPubDbContext dbContext) : Repository<Tag>(dbContext), ITagRepository
{
    public async Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names)
    {
        return await dbContext.Tags.Where(t => names.Contains(t.Name)).AsNoTracking().ToListAsync();
    }

    public async Task<Tag> AddAsync(string name)
    {
        var tag = new Tag { Name = name };
        await dbContext.Tags.AddAsync(tag);
        return tag;
    }
}