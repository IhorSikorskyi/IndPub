using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;

namespace IndPubBack.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> GetUserInfoAsync(Guid authorId);
    Task<UserInfoResponse> UpdateUserInfoAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> DeleteAccountAsync(Guid userId, Guid? targetUserId);
}