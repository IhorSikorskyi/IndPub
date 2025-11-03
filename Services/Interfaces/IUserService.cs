using IndPubBack.DTO.Responses;
using IndPubBack.DTO.Requests;

namespace IndPubBack.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> UpdateAccessTokenAsync(AccessTokenRequest request, string refreshToken);

        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
        Task<UserInfoResponse> UpdateUserInfoAsync(string accessToken, UpdateProfileRequest request);
    }
}