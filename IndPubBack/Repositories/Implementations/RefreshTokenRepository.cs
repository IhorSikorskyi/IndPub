using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations;

public class RefreshTokenRepository(Connected dbContext) : Repository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    private readonly DateTime _tokenExpiryThreshold = DateTime.UtcNow.AddDays(-5);
    public async Task RevokeAllTokensForUserAsync(Guid userId)
    {
        var tokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task RevokeTokenForUserAsync(Guid tokenId)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == tokenId && rt.RevokedAt == null) ?? throw new NotFoundException("Token not found");

        token.RevokedAt = DateTime.UtcNow;
    }

    public async Task RemoveOldTokensAsync(CancellationToken cancellationToken = default)
    {
        var oldTokens = await dbContext.RefreshTokens
            .Where(rt => rt.RefreshTokenExpiry < _tokenExpiryThreshold)
            .ToListAsync(cancellationToken);

        dbContext.RefreshTokens.RemoveRange(oldTokens);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByHashAsync(string hash)
    {
        return await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rf => rf.RefreshTokenHash == hash);
    }
}