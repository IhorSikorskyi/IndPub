using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> GetUserInfoAsync(Guid authorId);
    Task<UserInfoResponse> UpdateUserInfoAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> DeleteAccountAsync(Guid userId, Guid targetUserId);
}