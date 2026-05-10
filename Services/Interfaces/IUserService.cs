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
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
        Task<UserInfoResponse> UpdateUserInfoAsync(string accessToken, UpdateProfileRequest request); Task<bool> DeleteAccountAsync(string accessToken);

        // Публічний профіль автора
        Task<UserInfoResponse> GetAuthorProfileAsync(Guid authorId);

        // Підписки
        Task SubscribeAsync(Guid authorId, Guid userId);
        Task UnsubscribeAsync(Guid authorId, Guid userId);

        // Бібліотека
        Task<IList<BookResponse>> GetLibraryAsync(string accessToken);
    }
}