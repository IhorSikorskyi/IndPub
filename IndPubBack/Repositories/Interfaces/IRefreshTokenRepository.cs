using IndPubBack.Entities;

namespace IndPubBack.Repositories.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task RevokeAllTokensForUserAsync(Guid userId);
    Task<bool> RevokeTokenForUserAsync(Guid tokenId);
    Task RemoveOldTokensAsync(CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByHashAsync(string hash);
}