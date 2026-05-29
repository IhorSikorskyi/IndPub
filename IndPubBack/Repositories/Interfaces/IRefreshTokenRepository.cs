using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task RevokeAllTokensForUserAsync(Guid userId);
    Task RevokeTokenForUserAsync(Guid tokenId);
    Task RemoveOldTokensAsync();
    Task<RefreshToken?> GetByHashAsync(string hash);
}