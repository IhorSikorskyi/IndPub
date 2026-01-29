using Microsoft.EntityFrameworkCore;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations
{
    public class UserRepository(Connected _context) : Repository<User>(_context), IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByLoginAsync(string login)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> ExpireRefreshTokenAsync(Guid userId)
        {
            var affectedRows = await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.RefreshTokenExpiry, DateTime.UtcNow));

            return affectedRows > 0;
        }
    }
}