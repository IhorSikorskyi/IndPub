using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using System.Net;

namespace IndPubBack.Services.Implementations;

public class LibraryService(ILibraryRepository libraryRepository, IBookRepository bookRepository, IUserRepository userRepository) : ILibraryService
{
    public async Task<UserActivitiesResponse> GetLibraryAsync(Guid userId, LibraryListRequest request)
    {
        var data = await libraryRepository.GetCursorPageAsync(userId, request.Status, request.Cursor, request.PageSize);

        return new UserActivitiesResponse
        {
            Library = data.Select(e => new LibraryEntryResponse
            {
                BookId = e.BookId,
                Title = e.Book.Title,
                CoverImageUrl = e.Book.CoverImageUrl,
                UpdatedDate = e.Book.UpdatedDate,
                ChapterCount = e.Book.Chapters.Count,
                Status = e.Status
            }).ToList()
        };
    }

    public async Task<bool> AddToLibraryAsync(Guid bookId, Guid userId)
    {
        if(!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if(!await bookRepository.IsExistAsync(bookId))
        {
            throw new NotFoundException("Book not found");
        }

        if (await IsBookInLibraryAsync(userId, bookId))
        {
            throw new ConflictException("Book already in library");
        }

        var libraryEntry = new LibraryEntry
        {
            UserId = userId,
            BookId = bookId
        };

        await libraryRepository.AddAsync(libraryEntry);

        return true;
    }

    public async Task<bool> RemoveFromLibraryAsync(Guid bookId, Guid userId)
    {
        if (!await IsBookInLibraryAsync(userId, bookId))
        {
            throw new NotFoundException("Book not in library");
        }

        return await libraryRepository.DeleteFromLibraryAsync(userId, bookId);
    }

    public async Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId)
    {
        return await libraryRepository.IsBookInLibraryAsync(userId, bookId);
    }

    public async Task<bool> UpdateLibraryEntryStatusAsync(Guid userId, LibraryEntryRequest request)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        if (!await bookRepository.IsExistAsync(request.BookId))
        {
            throw new NotFoundException("Book not found");
        }

        if (!await IsBookInLibraryAsync(userId, request.BookId))
        {
            throw new NotFoundException("Book not in library");
        }

        return await libraryRepository.UpdateLibraryEntryStatusAsync(userId, request.BookId, request.Status);
    }
}