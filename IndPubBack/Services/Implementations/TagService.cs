using IndPubBack.DTOs.Requests;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<List<BookTag>> GetOrCreateBookTagsAsync(Guid bookId, List<CreateBookTagRequest> tagRequests)
    {
        var requestedNames = tagRequests.Select(t => t.Name).Distinct().ToList();

        var existingTags = await tagRepository.GetByNamesAsync(requestedNames);

        var existingNames = existingTags.Select(t => t.Name).ToHashSet();

        var missingNames = requestedNames.Where(name => !existingNames.Contains(name));
        var newTags = new List<Tag>();
        foreach (var name in missingNames)
        {
            newTags.Add(await tagRepository.AddAsync(name));
        }

        var allTags = existingTags.Concat(newTags);
        return [.. allTags.Select(tag => new BookTag { BookId = bookId, TagId = tag.Id })];
    }
}