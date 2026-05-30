namespace IndPubBack.Services.Interfaces;

public interface IBookInteractionService
{
    Task<bool> LikeBookInteractionAsync(Guid bookId, Guid userId);
    //TODO: Add methods to manage book views
}