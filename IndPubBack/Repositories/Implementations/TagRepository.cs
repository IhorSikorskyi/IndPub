using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class TagRepository(IndPubDbContext dbContext) : Repository<Tag>(dbContext), ITagRepository
{
    public Task<Tag?> GetByNameAsync(string name)
    {
        return dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<Tag> AddAsync(string name)
    {
        var tag = new Tag { Name = name };
        await dbContext.Tags.AddAsync(tag);
        await dbContext.SaveChangesAsync();
        return tag;
    }
}