using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken);
        Task<bool> LogoutAsync(string accessToken, string refreshToken);
    }
}