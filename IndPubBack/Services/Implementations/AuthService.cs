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
using SecurityException = IndPubBack.Exceptions.SecurityException;

namespace IndPubBack.Services.Implementations;

public class AuthService(
    IConfiguration configuration, 
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordValidationService passwordValidationService,
    IEntityValidationService entityValidationService)
    : IAuthService
{
    private static readonly string Check = "Invalid access token.";

    #region Authorization

    public async Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> 
        RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Length < 3
                                                     || string.IsNullOrWhiteSpace(request.Email) ||
                                                     !request.Email.Contains('@'))
        {
            throw new ValidationException("Invalid login or email.");
        }

        var userWithLoginExists = await entityValidationService.IsUserExistsAsync(request.Login);
        var userWithEmailExists = await entityValidationService.IsUserExistsAsync(request.Email);
        
        if (userWithLoginExists || userWithEmailExists)
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
            PasswordHash = null! // Will be set after generation
        };

        user.PasswordHash = HashPassword(user, request.Password);

        string rawToken = GenerateRefreshToken();
        string hashedRefreshToken = HashRefreshToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            RefreshTokenHash = hashedRefreshToken,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(15)
        };

        await userRepository.AddAsync(user);
        await refreshTokenRepository.AddAsync(refreshToken);

        var accessToken = await CreateAccessTokenResponseAsync(user, rawToken, refreshToken.RefreshTokenExpiry);

        return (accessToken, rawToken, refreshToken.RefreshTokenExpiry);
    }

    public async Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> 
        LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByLoginAsync(request.LoginOrEmail)
                   ?? await userRepository.GetByEmailAsync(request.LoginOrEmail) ?? throw new InvalidCredentialsException("Invalid user or password.");

        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException("Invalid user or password.");
        }

        string rawToken = GenerateRefreshToken();
        string hashedRefreshToken = HashRefreshToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            RefreshTokenHash = hashedRefreshToken,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(15)
        };

        await refreshTokenRepository.AddAsync(refreshToken);

        var accessToken = await CreateAccessTokenResponseAsync(user, rawToken, refreshToken.RefreshTokenExpiry);

        return (accessToken, rawToken, refreshToken.RefreshTokenExpiry);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await ValidateUserRefreshTokenAsync(refreshToken);

        await refreshTokenRepository.RevokeTokenForUserAsync(token.Id);
    }

    public async Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> UpdateAccessTokenAsync(
        string accessToken, string refreshToken)
    {
        var userId = GetUserIdFromExpiredToken(accessToken)
                     ?? throw new UnauthorizedException(Check);

        var user = await userRepository.GetByIdAsync(userId)
                   ?? throw new UnauthorizedException(Check);

        var oldRefreshToken = await ValidateUserRefreshTokenAsync(refreshToken);

        await refreshTokenRepository.RevokeTokenForUserAsync(oldRefreshToken.Id);

        string rawToken = GenerateRefreshToken();
        string hashedRefreshToken = HashRefreshToken(rawToken);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            RefreshTokenHash = hashedRefreshToken,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(15),
            ReplacedByTokenId = oldRefreshToken.Id
        };

        var newAccessToken = await CreateAccessTokenResponseAsync(user, rawToken, newRefreshToken.RefreshTokenExpiry);

        await refreshTokenRepository.AddAsync(newRefreshToken);

        return (newAccessToken, rawToken, newRefreshToken.RefreshTokenExpiry);
    }

    #endregion
    
    #region Helpers

    private async Task<UserResponse> CreateAccessTokenResponseAsync(User user, string rawToken, DateTime refreshTokenExpiry)
    {
        return new UserResponse
        {
            RefreshToken = rawToken,
            RefreshTokenExpiry = refreshTokenExpiry,
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

    private async Task<RefreshToken> ValidateUserRefreshTokenAsync(string rawRefreshToken)
    {
        var hashRefreshToken = HashRefreshToken(rawRefreshToken);

        var token = await refreshTokenRepository.GetByHashAsync(hashRefreshToken)
                    ?? throw new NotFoundException("Invalid refresh token");

        if (token.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeAllTokensForUserAsync(token.UserId);
            throw new SecurityException("Suspicious activity. Please try logging in again.");
        }

        if (token.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new ValidationException("Refresh token expired.");
        }

        return token;
    }

    private Guid? GetUserIdFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:AccessToken")!)),
            ValidateIssuer = true,
            ValidIssuer = configuration.GetValue<string>("AppSettings:Issuer"),
            ValidateAudience = true,
            ValidAudience = configuration.GetValue<string>("AppSettings:Audience"),
            ValidateLifetime = false
        };

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(accessToken, tokenValidationParameters, out _);

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
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
    
    private static string HashRefreshToken(string refreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }

    #endregion
}
