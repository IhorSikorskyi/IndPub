using IndPubBack.Data;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations;

public class UnitOfWork(IndPubDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() 
        => dbContext.SaveChangesAsync();
}