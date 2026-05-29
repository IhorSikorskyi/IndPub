using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> RegisterAsync(RegisterRequest request);
        Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> LoginAsync(LoginRequest request);
        Task<(UserResponse response, string refreshToken, DateTime refreshTokenExpiry)> UpdateAccessTokenAsync(string accessToken, string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}