namespace IndPubBack.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}