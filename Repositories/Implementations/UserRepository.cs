using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user!.Role.ToString();
        }

        public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids)
        {
            return await context.Users
                    .Where(u => ids.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();
        }
    }
}