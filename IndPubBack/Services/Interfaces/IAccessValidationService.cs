namespace IndPubBack.Services.Interfaces;

public interface IAccessValidationService
{
    Task<bool> IsUserIsAuthorAsync(Guid userId, Guid bookId);
    Task<bool> IsUserIsModeratorAsync(Guid userId);
}