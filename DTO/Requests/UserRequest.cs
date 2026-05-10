using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTO.Requests
{
    // TODO: Add CoverImage and Bio to RegisterRequest and UpdateProfileRequest
    public class RegisterRequest
    {
        public required string Login { get; set; }

        public required string Email { get; set; }

        [MinLength(1)]
        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }
    }

    public class LoginRequest
    {
        public required string LoginOrEmail { get; set; }

        public required string Password { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? Email { get; set; }

        public string? CurrentPassword { get; set; }

        [MinLength(1)]
        public string? NewPassword { get; set; }

        public string? ConfirmNewPassword { get; set; }

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
