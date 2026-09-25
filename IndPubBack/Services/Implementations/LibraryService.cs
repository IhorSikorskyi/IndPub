using IndPubBack.DTOs.Requests.Library;
using IndPubBack.DTOs.Responses.Library;
using IndPubBack.DTOs.Responses.User;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class LibraryService(
    ILibraryRepository libraryRepository,
    IEntityValidationService entityValidationService,
    IUnitOfWork unitOfWork) : ILibraryService
{
    public async Task<UserActivitiesResponse> GetLibraryAsync(Guid userId, LibraryListRequest request)
    {
        var data = await libraryRepository.GetCursorPageAsync(userId, request.Status, request.Cursor, request.PageSize);

        return new UserActivitiesResponse
        {
            Library = [..data.Select(e => new LibraryEntryResponse
            {
                BookId = e.BookId,
                Title = e.Book.Title,
                CoverImageUrl = e.Book.CoverImageUrl,
                UpdatedDate = e.Book.UpdatedDate,
                ChapterCount = e.Book.Chapters.Count,
                Status = e.Status
            })]
        };
    }

    public async Task<bool> AddToLibraryAsync(Guid bookId, Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true
            : throw new NotFoundException("Book not found");

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
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromLibraryAsync(Guid bookId, Guid userId)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        _ = await entityValidationService.IsBookExistsAsync(bookId) ? true
            : throw new NotFoundException("Book not found");

        var libraryEntry = await libraryRepository.GetByIdAsync(userId, bookId) ??
                           throw new NotFoundException("This book is not in library");

        libraryRepository.DeleteFromLibrary(libraryEntry);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsBookInLibraryAsync(Guid userId, Guid bookId)
    {
        return await libraryRepository.IsBookInLibraryAsync(userId, bookId);
    }

    public async Task<bool> UpdateLibraryEntryStatusAsync(Guid userId, LibraryEntryRequest request)
    {
        _ = await entityValidationService.IsUserExistsAsync(userId) ? true
            : throw new NotFoundException("User not found");

        _ = await entityValidationService.IsBookExistsAsync(request.BookId) ? true
            : throw new NotFoundException("Book not found");

        var libraryEntry = await libraryRepository.GetByIdAsync(userId, request.BookId) ??
                           throw new NotFoundException("This book is not in library");

        libraryRepository.UpdateLibraryEntryStatus(libraryEntry, request.Status);
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}