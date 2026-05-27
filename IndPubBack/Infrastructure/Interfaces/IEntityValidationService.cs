namespace IndPubBack.Infrastructure.Interfaces;

public interface IEntityValidationService
{
    Task EnsureUserExistsAsync(Guid userId);
    Task EnsureBookExistsAsync(Guid bookId);
}