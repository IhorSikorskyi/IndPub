using IndPubBack.DTO.Responses;
using IndPubBack.DTO.Requests;

namespace IndPubBack.Services.Interfaces
{
    // TODO: Implement save CoverImages to Azure Blob Storage and update CoverImageUrl to the URL and save to DB
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken);
        Task<bool> LogoutAsync(string accessToken, string refreshToken);

        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
        Task<UserInfoResponse> UpdateUserInfoAsync(string accessToken, UpdateProfileRequest request);

        //TODO: Add methods for password reset, email confirmation, etc.

        //TODO: Consider adding methods for role management, if needed in the future.

        //TODO: Add methods for user account deletion, if required by the application.
    }
}