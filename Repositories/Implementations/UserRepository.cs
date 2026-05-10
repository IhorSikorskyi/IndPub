using Microsoft.EntityFrameworkCore;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations
{
    public class UserRepository(Connected context) : Repository<User>(context), IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByLoginAsync(string login)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> ExpireRefreshTokenAsync(Guid userId)
        {
            var affectedRows = await context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.RefreshTokenExpiry, DateTime.UtcNow));

            return affectedRows > 0;
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            return await context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }
    }
}