using IndPubBack.DTO.Responses;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class LibraryService(IConfiguration configuration, ILibraryRepository libraryRepository) : ILibraryService
{
    #region Library

    //TODO: Implement library retrieval with necessary data aggregation, filtering, and formatting
    public async Task<IList<BookResponse>> GetLibraryAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implement adding books to library with proper authorization checks and data handling
    public async Task<bool> AddToLibraryAsync(Guid bookId, Guid userId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implement removing books from library with proper authorization checks and data handling
    public async Task<bool> RemoveFromLibraryAsync(Guid bookId, Guid userId)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Helpers

    // I just want here something
    // I already know for what this region is.

    #endregion
}