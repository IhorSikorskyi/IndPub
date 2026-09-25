using IndPubBack.DTOs.Requests.User;
using IndPubBack.DTOs.Responses.User;

namespace IndPubBack.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> GetUserInfoAsync(Guid authorId);
    Task<UserInfoResponse> UpdateUserInfoAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> DeleteAccountAsync(Guid userId, Guid targetUserId);
}