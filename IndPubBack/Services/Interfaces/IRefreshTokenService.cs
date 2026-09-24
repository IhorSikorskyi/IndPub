using IndPubBack.Entities;

namespace IndPubBack.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<(string, DateTime)> CreateRefreshTokenAsync(Guid userId, Guid? replacesTokenId = null);

    Task<RefreshToken> ValidateUserRefreshTokenAsync(string rawRefreshToken);

    Task<bool> RevokeRefreshTokenAsync(Guid tokenId);
}