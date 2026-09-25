using IndPubBack.DTOs.Requests.User;
using IndPubBack.DTOs.Responses.User;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IndPubBack.Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService,
    IPasswordValidationService passwordValidationService)
    : IAuthService
{

    #region Authorization

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {

        if (await userRepository.IsExistByLoginOrEmailAsync(request.Email) 
            || await userRepository.IsExistByLoginOrEmailAsync(request.Login))
        {
            throw new InvalidOperationException("User with this email or login already exists.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new InvalidOperationException("Passwords do not match.");
        }

        passwordValidationService.EnsurePasswordComplex(request.Password);

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            PasswordHash = null! // Will be set after generation
        };

        user.PasswordHash = HashPassword(user, request.Password);

        var (refreshTokenHash, expiry) = await refreshTokenService.CreateRefreshTokenAsync(user.Id);
        var accessToken = jwtTokenGenerator.GenerateToken(user);

        await userRepository.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        var response = MapUserResponse(refreshTokenHash, expiry, accessToken);

        return response;
    }

    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByLoginAsync(request.LoginOrEmail)
                   ?? await userRepository.GetByEmailAsync(request.LoginOrEmail) 
                   ?? throw new InvalidCredentialsException("Invalid user or password.");

        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException("Invalid user or password.");
        }

        var (refreshTokenHash, expiry) = await refreshTokenService.CreateRefreshTokenAsync(user.Id);
        var accessToken = jwtTokenGenerator.GenerateToken(user);

        var response = MapUserResponse(refreshTokenHash, expiry, accessToken);

        return response;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await refreshTokenService.ValidateUserRefreshTokenAsync(refreshToken);

        await refreshTokenService.RevokeRefreshTokenAsync(token.Id);
    }

    public async Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken)
    {
        var token = await refreshTokenService.ValidateUserRefreshTokenAsync(refreshToken);

        var user = await userRepository.GetByIdAsync(token.UserId)
                   ?? throw new InvalidOperationException("User not found.");

        var newAccessToken = jwtTokenGenerator.GenerateToken(user);

        var response = MapUserResponse(token.RefreshTokenHash, token.RefreshTokenExpiry, newAccessToken);

        return response;
    }

    #endregion

    #region Helpers

    private static UserResponse MapUserResponse(string refreshTokenHash, DateTime refreshTokenExpiry, AccessTokenResponse accessToken)
    {
        return new UserResponse
        {
            RefreshToken = refreshTokenHash,
            RefreshTokenExpiry = refreshTokenExpiry,
            AccessToken = accessToken
        };
    }

    private static string HashPassword(User user, string password)
    {
        return new PasswordHasher<User>().HashPassword(user, password);
    }

    #endregion
}
