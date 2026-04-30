using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using IndPubBack.Models;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Exceptions;

namespace IndPubBack.Services.Implementations;

public class BookService(IConfiguration _configuration, IBookRepository _bookRepository) : IBookService
{
    public async Task<BookCreateResponse> CreateBookAsync(BookCreateRequest request)
    {
        if (IsTitleExist(request.Title))
        {
            throw new ConflictException("Book with the same name already exist");
        }

        string coverImageUrl = string.Empty;

        if (request.CoverImage is not null)
        {
            coverImageUrl = SaveCoverImage(request.CoverImage);
        }

        var book = new Book
        {
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            CoverImageUrl = string.IsNullOrWhiteSpace(coverImageUrl) ? null : coverImageUrl,
            PublishedDate = request.PublishedDate,
            UpdatedDate = request.PublishedDate,
            ChapterCount = request.Chapters.Count,
            Language = request.Language,
            Status = request.Status,
        };

        throw new NotImplementedException();
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
        var check = _bookRepository.HasTitleAsync(title).Result;

        if (check)
        {
            return true;
        }

        return false;
    }

    private static string SaveCoverImage(IFormFile image)
    {
        if (!CoverImageValidation(image))
        {
            throw new ValidationException("Invalid image");
        }

        throw new NotImplementedException();
    }

    private static bool CoverImageValidation(IFormFile image)
    {
        throw new NotImplementedException();
    }
}