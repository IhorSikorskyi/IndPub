using System.IO;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;


namespace IndPubBack.Services.Implementations;

public class BookService(IConfiguration configuration, IBookRepository bookRepository, IBlobService blobService) : IBookService
{
    public async Task<BookCreateResponse> CreateBookAsync(BookCreateRequest request)
    {
        if (IsTitleExist(request.Title))
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
        };

        if (request.CoverImage is not null)
        {
            if (!CoverImageValidation(request.CoverImage))
            {
                throw new ValidationException("Invalid image");
            }

            string folder = configuration["AzureStorage:BookCoversFolder"]!;

            string coverImageUrl = await blobService.UploadBlobAsync(folder, request.CoverImage, book.Id);

            book.CoverImageUrl = coverImageUrl;
        }

        var response = new BookCreateResponse
        {
            BookId = book.Id,
            Title = book.Title
        };

        return response;
    }

    // TODO: Implement UpdateBookAsync verification for being author of the book
    public async Task<BookUpdateResponse> UpdateBookAsync(BookUpdateRequest request, Guid userId)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement DeleteBookAsync verification for being author of the book or an admin
    public async Task<BookDeleteResponse> DeleteBookAsync(BookDeleteRequest request, Guid userId)
    {
        throw new NotImplementedException();
    }

    private bool IsTitleExist(string title)
    {
        var check = bookRepository.HasTitleAsync(title).Result;

        if (check)
        {
            return true;
        }

        return false;
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
}