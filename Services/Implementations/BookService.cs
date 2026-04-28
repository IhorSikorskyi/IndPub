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
}