using IndPubBack.Infrastructure.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class TokenRevocationStore : ITokenRevocationStore
{
    public async Task<bool> IsRevokedAsync(string userId)
    {
        throw new NotImplementedException();
    }
}