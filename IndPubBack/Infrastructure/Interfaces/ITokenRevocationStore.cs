namespace IndPubBack.Infrastructure.Interfaces;

public interface ITokenRevocationStore
{
    Task<bool> IsRevokedAsync(string userId);
}