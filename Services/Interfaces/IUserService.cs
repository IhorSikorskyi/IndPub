using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces
{
    public interface IUserService
    {
        // Автентифікація
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken);
        Task<bool> LogoutAsync(string accessToken, string refreshToken);

        // Профіль
        Task<UserInfoResponse> GetUserInfoAsync(string? accessToken, Guid? authorId);
        Task<UserInfoResponse> UpdateUserInfoAsync(string accessToken, UpdateProfileRequest request);
        Task<bool> DeleteAccountAsync(string accessToken, Guid? targetUserId);

        // Підписки
        Task<IList<BookShortResponse>> GetSubscriptionListAsync(string accessToken);
        Task<bool> SubscribeAsync(Guid authorId, string accessToken);
        Task<bool> UnsubscribeAsync(Guid authorId, string accessToken);

        // Бібліотека
        Task<IList<BookResponse>> GetLibraryAsync(string accessToken);
        Task<bool> AddToLibraryAsync(Guid bookId, string accessToken);
        Task<bool> RemoveFromLibraryAsync(Guid bookId, string accessToken);
    }
}