using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class BookService(
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork,
    ITagService tagService,
    IImageService imageService,
    IEntityValidationService entityValidationService,
    IAccessValidationService accessValidationService) : IBookService
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const string BookCoversFolderKey = "AzureStorage:BookCoversFolder";

    #region CRUD
    public async Task<BookResponse> CreateBookAsync(BookCreateRequest request, Guid currentUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required");
        }

        ValidateAuthorsNumbers(request.AuthorIds);

        if (!request.AuthorIds.Contains(currentUserId))
        {
            throw new ForbiddenException("You can only create a book where you are listed as an author.");
        }

        if (await IsTitleExistAsync(request.Title))
        {
            throw new ConflictException("Book with the same name already exist");
        }

        var book = new Book
        {
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            CreatedAt = request.PublishedDate,
            UpdatedDate = request.PublishedDate,
            Language = request.Language,
            Status = request.Status,
            GenreId = request.GenreId,
            CategoryId = request.CategoryId,
            SubcategoryId = request.SubcategoryId
        };

        if (request.CoverImage is not null)
        {
            book.CoverImageUrl = await imageService.UploadImageAsync(
                request.CoverImage, BookCoversFolderKey, book.Id, MaxFileSize);
        }

        book.Chapters = [..request.Chapters.Select((c, index) => new Chapter
        {
            BookId = book.Id,
            Title = c.Title,
            Content = c.Content,
            ChapterNumber = index + 1,
        })];

        await Task.WhenAll(request.AuthorIds.Select(entityValidationService.IsUserExistsAsync));

        book.BookAuthors = [..request.AuthorIds.Select(authorId => new BookAuthor
        {
            BookId = book.Id,
            UserId = authorId
        })];

        if (request.Tags is not null && request.Tags.Count > 0)
        {
            var bookTags = new List<BookTag>();
            foreach (var tagRequest in request.Tags)
            {
                var tag = await tagService.GetOrCreateBookTagsAsync(book.Id, new List<CreateBookTagRequest> { tagRequest });
                bookTags.AddRange(tag);
            }
            book.BookTags = bookTags;
        }

        await bookRepository.AddAsync(book);
        await unitOfWork.SaveChangesAsync();

        return MapToBookResponse(book);
    }

    public async Task<BookResponse> UpdateBookAsync(BookUpdateRequest request, Guid bookId, Guid userId)
    {
        var book = await bookRepository.GetByIdAsync(bookId)
                   ?? throw new NotFoundException("Book not found");

        _ = await accessValidationService.IsUserIsModeratorAsync(userId) ? true 
            : throw new AccessViolationException("You are not an author or a moderator of this book.");

        _ = await bookRepository.IsUserAuthorAsync(userId, bookId) ? true
            : throw new AccessViolationException("You are not an author of this book.");

        book.UpdatedDate = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != book.Title)
        {
            if (await IsTitleExistAsync(request.Title))
            {
                throw new ConflictException("Book with the same name already exist");
            }

            book.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            book.Description = request.Description;
        }

        if (request.CoverImage is not null)
        {
            book.CoverImageUrl = await imageService.UploadImageAsync(
                request.CoverImage, BookCoversFolderKey, book.Id, MaxFileSize);
        }

        book.Status = request.Status;

        if (request.AuthorIds is not null)
        {
            ValidateAuthorsNumbers(request.AuthorIds);
            await Task.WhenAll(request.AuthorIds.Select(entityValidationService.IsUserExistsAsync));
            book.BookAuthors = [..request.AuthorIds.Select(authorId => new BookAuthor
            {
                BookId = book.Id,
                UserId = authorId
            })];
        }

        if (request.Tags is not null && request.Tags.Count > 0)
        {
            var bookTags = new List<BookTag>();
            foreach (var tagRequest in request.Tags)
            {
                var tag = await tagService.GetOrCreateBookTagsAsync(book.Id, new List<CreateBookTagRequest> { tagRequest });
                bookTags.AddRange(tag);
            }
            book.BookTags = bookTags;
        }

        bookRepository.Update(book);
        await unitOfWork.SaveChangesAsync();

        return MapToBookResponse(book);
    }

    public async Task<bool> DeleteBookAsync(Guid bookId, Guid userId)
    {
        var book = await bookRepository.GetByIdAsync(bookId) 
                   ?? throw new NotFoundException("Book not found.");

        _ = await accessValidationService.IsUserIsModeratorAsync(userId) ? true
            : throw new AccessViolationException("You are not an author or a moderator of this book.");

        _ = await bookRepository.IsUserAuthorAsync(userId, bookId) ? true
            : throw new AccessViolationException("You are not an author of this book.");

        bookRepository.Delete(book);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<BookResponse> GetBookByIdAsync(Guid bookId)
    {
        var book = await bookRepository.GetByIdAsync(bookId) ?? throw new NotFoundException("Book not found.");

        return MapToBookResponse(book);
    }

    #endregion

    #region Helpers
    private async Task<bool> IsTitleExistAsync(string title)
    {
        return await bookRepository.HasTitleAsync(title);
    }

    private static void ValidateAuthorsNumbers(List<Guid> authorIds)
    {
        if (authorIds is null || authorIds.Count == 0 || authorIds.Count > 4)
        {
            throw new ValidationException("A book must have at least one author and no more than four authors.");
        }
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
            GenreName = book.Genre.Name,
            CategoryName = book.Category.Name,
            SubcategoryName = book.Subcategory.Name,
            ChapterCount = book.Chapters.Count,
            Rating = book.Rating,
            Authors = [..book.BookAuthors.Select(ba => new AuthorResponse
            {
                Id = ba.UserId,
                Login = ba.User.Login,
                ProfilePictureUrl = ba.User.ProfilePictureUrl
            })],
            Tags = [..book.BookTags.Select(bt => bt.Tag.Name)],
            Chapters = [..book.Chapters.Select(c => new ChapterShortResponse
            {
                ChapterId = c.Id,
                Title = c.Title,
                ChapterNumber = c.ChapterNumber
            })]
        };
    }

    #endregion
}