using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Repositories.Implementations
{
    public class Repository<T>(DbContext dbContext) : IRepository<T> where T : BaseEntity
    {
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<bool> IsExistAsync(Guid id)
        {
            return await dbContext.Set<T>().AnyAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public virtual void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }
    }
}