using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IndPubBack.Infrastructure.Interfaces;

namespace IndPubBack.Services.Implementations;

public class AuthService(
    IConfiguration configuration, 
    IUserRepository userRepository,
    IPasswordValidationService passwordValidationService)
    : IAuthService
{
    private static readonly string Check = "Invalid access token.";

    #region Authorization

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Length < 3
                                                     || string.IsNullOrWhiteSpace(request.Email) ||
                                                     !request.Email.Contains('@'))
        {
            throw new ValidationException("Invalid login or email.");
        }

        var existingUser = await userRepository.GetByLoginAsync(request.Login)
                           ?? await userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new ConflictException("User with the same username or email already exists.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new ValidationException("Passwords do not match.");
        }

        passwordValidationService.EnsurePasswordComplex(request.Password);

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            PasswordHash = null!, // Will be set after hashing
            RefreshToken = null! // Will be set after generation
        };

        user.PasswordHash = HashPassword(user, request.Password);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await userRepository.AddAsync(user);

        return await CreateAccessTokenResponseAsync(user);
    }

    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByLoginAsync(request.LoginOrEmail)
                   ?? await userRepository.GetByEmailAsync(request.LoginOrEmail) ?? throw new InvalidCredentialsException("Invalid user or password.");

        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException("Invalid user or password.");
        }

        if (!ValidateRefreshToken(user, user.RefreshToken))
        {
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await userRepository.UpdateAsync(user);
        }

        return await CreateAccessTokenResponseAsync(user);
    }

    public async Task<bool> LogoutAsync(Guid userId, string refreshToken)
    {
        var user = await userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedException(Check);

        if (!ValidateRefreshToken(user, refreshToken))
        {
            throw new UnauthorizedException("Invalid refresh token. Please log in again.");
        }

        var tokenExpiredSuccessfully = await userRepository.ExpireRefreshTokenAsync(user.Id);

        if (!tokenExpiredSuccessfully)
        {
            throw new UnauthorizedException("Failed to expire refresh token. User may have been deleted.");
        }

        return true;
    }

    public async Task<UserResponse> UpdateAccessTokenAsync(Guid userId, string refreshToken)
    {
        var user = await userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedException(Check);

        if (!ValidateRefreshToken(user, refreshToken))
        {
            throw new UnauthorizedException("Invalid refresh token. Please log in again.");
        }

        return await CreateAccessTokenResponseAsync(user);
    }

    #endregion
    
    #region Helpers

    private async Task<UserResponse> CreateAccessTokenResponseAsync(User user)
    {

        return new UserResponse
        {
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiry = user.RefreshTokenExpiry,
            AccessToken = await CreateTokenAsync(user)
        };
    }

    private async Task<string> CreateTokenAsync(User user)
    {
        var role = await userRepository.GetUserRoleAsync(user.Id);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:AccessToken")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var accessToken = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(accessToken);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashPassword(User user, string password)
    {
        return new PasswordHasher<User>().HashPassword(user, password);
    }

    private static bool ValidateRefreshToken(User? user, string refreshToken)
    {
        if (user == null 
            || user.RefreshToken != refreshToken 
            || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }
    
    #endregion
}
