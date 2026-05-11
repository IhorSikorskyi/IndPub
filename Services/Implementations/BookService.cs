using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;


namespace IndPubBack.Services.Implementations;

public class BookService(
    IConfiguration configuration,
    IBookRepository bookRepository,
    IUserRepository userRepository,
    ITagRepository tagRepository,
    IGenreRepository genreRepository,
    IBlobService blobService) : IBookService
{
    #region CRUD
    public async Task<BookResponse> CreateBookAsync(BookCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required");
        }

        if (request.AuthorIds is null || request.AuthorIds.Count == 0)
        {
            throw new ValidationException("At least one author is required");
        }

        if (request.Chapters is null || request.Chapters.Count == 0)
        {
            throw new ValidationException("At least one chapter is required");
        }

        if (await IsTitleExistAsync(request.Title))
        {
            throw new ConflictException("Book with the same name already exist");
        }

        var book = new Book
        {
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            PublishedDate = request.PublishedDate,
            UpdatedDate = request.PublishedDate,
            ChapterCount = request.Chapters.Count,
            Language = request.Language,
            Status = request.Status,
            GenreId = request.GenreId
        };

        if (request.CoverImage is not null)
        {
            if (!CoverImageValidation(request.CoverImage))
            {
                throw new ValidationException("Invalid image");
            }

            string folder = configuration["AzureStorage:BookCoversFolder"]!;
            book.CoverImageUrl = await blobService.UploadBlobAsync(folder, request.CoverImage, book.Id);
        }

        book.Chapters = request.Chapters.Select((c, index) => new Chapter
        {
            Title = c.Title,
            Content = c.Content,
            ChapterNumber = index + 1,
            BookId = book.Id
        }).ToList();

        await CheckAuthorsExistenceAsync(request.AuthorIds);

        book.BookAuthors = request.AuthorIds.Select(authorId => new BookAuthor
        {
            BookId = book.Id,
            UserId = authorId
        }).ToList();

        if (request.Tags is not null && request.Tags.Count > 0)
        {
            var bookTags = new List<BookTag>();
            foreach (var tagRequest in request.Tags)
            {
                var tag = await tagRepository.GetByNameAsync(tagRequest.Name)
                          ?? await tagRepository.AddAsync(tagRequest.Name);
                bookTags.Add(new BookTag { BookId = book.Id, TagId = tag.Id });
            }
            book.BookTags = bookTags;
        }

        await bookRepository.AddAsync(book);

        return MapToBookResponse(book);
    }

    public async Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid bookId, Guid userId)
    {
        var book = await bookRepository.GetByIdAsync(bookId)
                   ?? throw new NotFoundException("Book not found");

        if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != book.Title)
        {
            if (await IsTitleExistAsync(request.Title))
            {
                throw new ConflictException("Book with the same name already exist");
            }

            book.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
            book.Description = request.Description;

        if (request.CoverImage is not null)
        {
            if (!CoverImageValidation(request.CoverImage))
            {
                throw new ValidationException("Invalid image");
            }

            string folder = configuration["AzureStorage:BookCoversFolder"]!;
            book.CoverImageUrl = await blobService.UploadBlobAsync(folder, request.CoverImage, book.Id);
        }

        book.UpdatedDate = DateTime.UtcNow;
        book.Status = request.Status;

        if (request.AuthorIds is not null)
        {
            await CheckAuthorsExistenceAsync(request.AuthorIds);
            book.BookAuthors = request.AuthorIds.Select(authorId => new BookAuthor
            {
                BookId = book.Id,
                UserId = authorId
            }).ToList();
        }

        if (request.Tags is not null && request.Tags.Count > 0)
        {
            var bookTags = new List<BookTag>();
            foreach (var tagRequest in request.Tags)
            {
                var tag = await tagRepository.GetByNameAsync(tagRequest.Name)
                          ?? await tagRepository.AddAsync(tagRequest.Name);
                bookTags.Add(new BookTag { BookId = book.Id, TagId = tag.Id });
            }
            book.BookTags = bookTags;
        }

        await bookRepository.UpdateAsync(book);

        return MapToBookResponse(book);
    }

    public async Task<bool> DeleteBookAsync(Guid bookId, Guid userId)
    {
        var role = await userRepository.GetUserRoleAsync(userId);
        var book = await bookRepository.GetByIdAsync(bookId) 
                   ?? throw new NotFoundException("Book not found.");

        if (!(role == "Admin" || book.BookAuthors.Any(ba => ba.UserId == userId)))
        {
            throw new UnauthorizedException("You are not allowed to delete this book");
        }

        await bookRepository.DeleteAsync(bookId);

        return true;
    }

    #endregion

    #region Receiving

    //TODO: Add pagination and filtering
    public async Task<BookResponse> GetBookByIdAsync(Guid bookId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetAllBooksAsync()
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByAuthorIdAsync(Guid authorId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByGenreAsync(Guid genreId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> GetBooksByTagsAsync(Guid tagId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add pagination and filtering
    public async Task<IList<BookShortResponse>> SearchBooksAsync(string query)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Interaction

    //TODO: Add possibility to like only published books and prevent authors from liking their own books
    public async Task<bool> LikeBookAsync(Guid bookId, Guid userId)
    {
        throw new NotImplementedException();
    }

    //TODO: Add possibility to unlike only published books and prevent authors from unliking their own books
    public async Task<bool> UnlikeBookAsync(Guid bookId, Guid userId)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Helpers
    private async Task<bool> IsTitleExistAsync(string title)
    {
        return await bookRepository.HasTitleAsync(title);
    }

    private async Task CheckAuthorsExistenceAsync(List<Guid> authorIds)
    {
        var existingUserIds = await userRepository.GetExistingIdsAsync(authorIds);
        var missingIds = authorIds.Except(existingUserIds).ToList();

        if (missingIds.Count > 0)
        {
            throw new NotFoundException($"Users not found: {string.Join(", ", missingIds)}");
        }
    }

    private static bool CoverImageValidation(IFormFile image)
    {
        if (image.Length == 0)
        {
            return false;
        }

        const long maxFileSize = 5 * 1024 * 1024;
        if (image.Length > maxFileSize)
        {
            return false;
        }

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };

        var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/jpg"
        };

        var extension = Path.GetExtension(image.FileName);

        return !string.IsNullOrWhiteSpace(extension)
            && allowedExtensions.Contains(extension)
            && !string.IsNullOrWhiteSpace(image.ContentType)
            && allowedContentTypes.Contains(image.ContentType);
    }

    private static BookResponse MapToBookResponse(Book book)
    {
        return new BookResponse
        {
            BookId = book.Id,
            Title = book.Title,
            Description = book.Description,
            CoverImageUrl = book.CoverImageUrl,
            UpdatedDate = book.UpdatedDate,
            Language = book.Language,
            Status = book.Status,
            ChapterCount = book.ChapterCount,
            Authors = book.BookAuthors.Select(ba => new AuthorResponse
            {
                Id = ba.UserId,
                Login = ba.User?.Login ?? string.Empty,
                ProfilePictureUrl = ba.User?.ProfilePictureUrl
            }).ToList(),
            Tags = book.BookTags?.Select(bt => bt.Tag?.Name ?? string.Empty).ToList() ?? [],
            Chapters = book.Chapters.Select(c => new ChapterShortResponse
            {
                Id = c.Id,
                Title = c.Title,
                ChapterNumber = c.ChapterNumber
            }).ToList()
        };
    }

    #endregion

}