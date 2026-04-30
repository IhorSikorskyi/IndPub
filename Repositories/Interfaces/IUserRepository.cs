using IndPubBack.Models;

namespace IndPubBack.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByLoginAsync(string login);
        Task<bool> ExpireRefreshTokenAsync(Guid userId);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
    }
}