namespace IndPubBack.Services.Interfaces;

public interface IBookInteractionService
{
    Task<bool> LikeBookAsync(Guid bookId, Guid userId);
    Task<bool> UnlikeBookAsync(Guid bookId, Guid userId);
}