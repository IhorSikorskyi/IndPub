using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations
{
    public class Repository<T>(DbContext context) : IRepository<T> where T : class
    {
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}