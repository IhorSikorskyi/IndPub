using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace IndPubBack.Repositories.Implementations
{
    public class UserRepository(Connected dbContext) : Repository<User>(dbContext), IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email)
        {
            return dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByLoginAsync(string login)
        {
            return dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> ExpireRefreshTokenAsync(Guid userId)
        {
            var affectedRows = await dbContext.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.RefreshTokenExpiry, DateTime.UtcNow));

            return affectedRows > 0;
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            return user!.Role.ToString();
        }

        public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids)
        {
            return await dbContext.Users
                    .Where(u => ids.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();
        }

        // This method is overridden to include all related entities of the User, which are necessary for the application logic.
        // But need to be careful with this method, because it can lead to performance issues if the user has a lot of related entities.
        // Consider using separate methods to get related entities if necessary.
        // Batter to use separate methods to get related entities if necessary,
        // and use this method only when you need to get all related entities of the user. 
        public override async Task<User?> GetByIdAsync(Guid id)
        {
            var data = await dbContext.Users
                .Include(u => u.Entries).ThenInclude(e => e.Book)
                .Include(u => u.BookLikes).ThenInclude(bl => bl.Book.Title)
                .Include(u => u.Reviews).ThenInclude(r => r.Book.Title)
                .Include(u => u.Comments).ThenInclude(c => c.Chapter) 
                .Include(u => u.Subscriptions).ThenInclude(s => s.User)
                .Include(u => u.Bookmarks).ThenInclude(b => b.Chapter)
                    .ThenInclude(c => c.Book)
                .FirstOrDefaultAsync(u => u.Id == id);

            return data;
        }

    }
}