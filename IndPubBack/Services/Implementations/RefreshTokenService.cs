using IndPubBack.DTOs.Responses;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace IndPubBack.Services.Implementations;

public class RefreshTokenService(
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : IRefreshTokenService
{
    public async Task<(string, DateTime)> CreateRefreshTokenAsync(Guid userId, Guid? replacesTokenId = null)
    {
        var rawToken = GenerateRefreshToken();

        var timeNow = DateTime.UtcNow;

        var refreshTokenEntity = new RefreshToken
        {
            RefreshTokenHash = HashRefreshToken(rawToken),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                Convert.ToDouble(configuration["RefreshTokenLifetime:Days"])),
            CreatedAt = timeNow
        };

        await refreshTokenRepository.AddAsync(refreshTokenEntity);

        if (replacesTokenId is not null)
        {
            var previous = await refreshTokenRepository.GetByIdAsync(replacesTokenId.Value);
            previous?.ReplacedByTokenId = refreshTokenEntity.Id;
        }

        await unitOfWork.SaveChangesAsync();

        return (refreshTokenEntity.RefreshTokenHash, refreshTokenEntity.RefreshTokenExpiry);
    }

    public async Task<RefreshToken> ValidateUserRefreshTokenAsync(string rawRefreshToken)
    {
        var hashRefreshToken = HashRefreshToken(rawRefreshToken);

        var token = await refreshTokenRepository.GetByHashAsync(hashRefreshToken)
                    ?? throw new NotFoundException("Invalid refresh token");

        if (token.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeAllTokensForUserAsync(token.UserId);
            throw new TokenReuseDetectedException("Suspicious activity. Please try logging in again.");
        }

        if (token.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new TokenExpiredException("Refresh token expired.");
        }

        return token;
    }

    public async Task<bool> RevokeRefreshTokenAsync(Guid tokenId)
    {
        var token = await refreshTokenRepository.GetByIdAsync(tokenId);

        if (token is null || token.RevokedAt is not null)
        {
            return false;
        }

        if (token.RevokedAt is not null)
        {
            throw new TokenReuseDetectedException("Token has already been revoked.");
        }

        token.RevokedAt = DateTime.UtcNow;
        var revoked = await refreshTokenRepository.RevokeTokenForUserAsync(tokenId);

        await unitOfWork.SaveChangesAsync();

        return revoked;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }
}