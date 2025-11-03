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
    }
}