using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class TagRepository(Connected context) : Repository<Tag>(context), ITagRepository
{
    public Task<Tag?> GetByNameAsync(string name)
    {
        return context.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<Tag> AddAsync(string name)
    {
        var tag = new Tag { Name = name };
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();
        return tag;
    }
}