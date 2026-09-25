using IndPubBack.DTOs.Requests.User;
using IndPubBack.DTOs.Responses.User;

namespace IndPubBack.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync(string refreshToken);
        Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken);
    }
}