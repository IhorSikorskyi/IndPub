namespace IndPubBack.Services.Interfaces;

public interface IBookInteractionService
{
    Task<bool> LikeBookAsync(Guid bookId, Guid userId);
    Task<bool> UnLikeBookAsync(Guid bookId, Guid userId);

    //TODO: Add methods to manage book views
}