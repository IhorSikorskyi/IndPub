using Azure.Core;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Implementations;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IndPubBack.Services.Implementations;

public class UserService(
    IConfiguration configuration,
    IUserRepository userRepository,
    IBlobService blobService,
    IPasswordValidationService passwordValidationService,
    IImageValidationService imageValidationService)
    : IUserService
{
    private static readonly string Check = "Invalid access token.";
    const long MaxFileSize = 2 * 1024 * 1024;

    #region Profile

    public async Task<UserInfoResponse> GetUserInfoAsync(Guid authorId)
    {
        var user = await userRepository.GetByIdAsync(authorId)
                   ?? throw new NotFoundException("User not found.");

        return MapToUserInfoResponse(user);
    }

    public async Task<UserInfoResponse> UpdateUserInfoAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedException(Check);

        UpdateBio(user, request.Bio);
        await UpdateAvatarAsync(user, request.ProfilePicture);
        await UpdateLoginAsync(user, request.Login);
        await UpdateEmailAsync(user, request.Email);
        UpdatePassword(user, request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword);

        await userRepository.UpdateAsync(user);

        return new UserInfoResponse
        {
            Login = user.Login,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<bool> DeleteAccountAsync(Guid userId, Guid? targetUserId)
    {
        var idToDelete = targetUserId ?? userId;

        if (idToDelete != userId)
        {
            var role = await userRepository.GetUserRoleAsync(userId);
            if (role != "Admin")
            {
                throw new UnauthorizedException("You are not allowed to delete this user.");
            }
        }

        await userRepository.DeleteAsync(idToDelete);

        return true;
    }

    #endregion

    #region Helpers

    private async Task UpdateAvatarAsync(User user, IFormFile? avatar)
    {
        if (avatar is null)
        {
            return;
        }

        if (!imageValidationService.ValidateImage(avatar, MaxFileSize))
        {
            throw new ValidationException("Invalid image");
        }

        string folder = configuration["AzureStorage:ProfilePicturesFolder"]!;
        string avatarUrl = await blobService.UploadBlobAsync(folder, avatar, user.Id);
        user.ProfilePictureUrl = avatarUrl;
    }

    private async Task UpdateLoginAsync(User user, string? login)
    {
        if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
        {
            return;
        }

        var loginExists = await userRepository.GetByLoginAsync(login);
        if (loginExists != null && loginExists.Id != user.Id)
        {
            throw new ConflictException("Login already in use.");
        }

        user.Login = login;
    }

    private async Task UpdateEmailAsync(User user, string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var emailExists = await userRepository.GetByEmailAsync(email);
        if (emailExists != null && emailExists.Id != user.Id)
        {
            throw new ConflictException("Email already in use.");
        }

        user.Email = email;
    }

    private void UpdatePassword(User user,
        string? currentPassword,
        string? newPassword,
        string? confirmNewPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(confirmNewPassword))
        {
            return;
        }

        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException("Invalid password.");
        }

        if (newPassword != confirmNewPassword)
        {
            throw new ValidationException("New passwords do not match.");
        }

        passwordValidationService.EnsurePasswordComplex(newPassword);

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
    }

    private static void UpdateBio(User user, string? bio)
    {
        if (!string.IsNullOrWhiteSpace(bio))
        {
            user.Bio = bio;
        }
    }

    private static UserInfoResponse MapToUserInfoResponse(User user)
    {
        return new UserInfoResponse
        {
            Login = user.Login,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            JoiningDate = user.JoiningDate,
            SubscribersCount = user.Subscribers.Count
        };
    }

    #endregion
}