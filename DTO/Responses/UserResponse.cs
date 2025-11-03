namespace IndPubBack.DTO.Responses
{
    public class UserResponse
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }

    public class UserInfoResponse
    {
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}