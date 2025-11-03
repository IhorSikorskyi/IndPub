using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using IndPubBack.Models;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Services.Implementations
{
    public class UserService(IConfiguration _configuration, IUserRepository _userRepository) : IUserService
    {
        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Length < 3
            || string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new ArgumentException("Invalid login or email.");

            var existingUser = await _userRepository.GetByLoginAsync(request.Login)
                                ?? await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ArgumentException("User with the same username or email already exists");

            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException("Passwords do not match.");

            if (!IsPasswordComplex(request.Password))
            {
                throw new ArgumentException(
                    "Password must be at least 8 characters long, " +
                    "contain at least one uppercase letter, one lowercase letter, " +
                    "one number and one special character.");
            }

            var user = new User
            {
                Login = request.Login,
                Email = request.Email,
                PasswordHash = null! // Will be set after hashing
            };

            user.PasswordHash = HashPassword(user, request.Password);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepository.AddAsync(user);

            return CreateAccessTokenResponse(user);
        }

        public async Task<UserResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.LoginOrEmail)
                       ?? await _userRepository.GetByEmailAsync(request.LoginOrEmail);
            if (user == null)
                throw new ArgumentException("Invalid User or Password");

            var verificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new ArgumentException("Invalid User or Password");
            if (!ValidateRefreshToken(user))
            {
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateAsync(user);
            }
            return CreateAccessTokenResponse(user);
        }

        public async Task<UserResponse> UpdateAccessTokenAsync(AccessTokenRequest request, string refreshToken)
        {
            var userId = GetUserIdFromClaims(request.AccessToken);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid Access Token");
            }

            if (!ValidateRefreshToken(user, refreshToken))
            {
                throw new ArgumentException("Invalid Refresh Token. Please log in again.");
            }

            if (!ValidateRefreshToken(user))
            {
                throw new ArgumentException("Refresh token is expired. Please log in again.");
            }

            return CreateAccessTokenResponse(user);
        }

        private Guid GetUserIdFromClaims(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new ArgumentException("Invalid Access Token");
            }
            return userId;
        }

        private UserResponse CreateAccessTokenResponse(User user)
        {
            return new UserResponse
            {
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry,
                AccessToken = CreateToken(user)
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:AccessToken")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var accessToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string HashPassword(User user, string password)
        {
            return new PasswordHasher<User>().HashPassword(user, password);
        }

        private static bool IsPasswordComplex(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;

                if (hasUpper && hasLower && hasDigit && hasSpecial)
                    return true;
            }

            return false;
        }

        private static bool ValidateRefreshToken(User user, string refreshToken)
        {
            if (user == null || user.RefreshToken != refreshToken)
                return false;

            return true;
        }

        private static bool ValidateRefreshToken(User user)
        {
            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var userId = GetUserIdFromClaims(accessToken);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            return new UserInfoResponse
            {
                Login = user.Login,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                JoiningDate = user.JoiningDate
            };
        }

        public async Task<UserInfoResponse> UpdateUserInfoAsync(string accessToken, UpdateProfileRequest request)
        {
            var userId = GetUserIdFromClaims(accessToken);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new ArgumentException("Invalid Access Token");

            if (!string.IsNullOrWhiteSpace(request.Bio))
                user.Bio = request.Bio;

            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl))
                user.ProfilePictureUrl = request.ProfilePictureUrl;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _userRepository.GetByEmailAsync(request.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                {
                    throw new ArgumentException("Email already in use.");
                }
                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                var verificationResult = new PasswordHasher<User>()
                    .VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    throw new ArgumentException("Invalid password.");
                }
                if (!string.IsNullOrWhiteSpace(request.NewPassword) && !string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                {
                    if (request.NewPassword != request.ConfirmNewPassword)
                    {
                        throw new ArgumentException("New password do not match.");
                    }

                    if (!IsPasswordComplex(request.NewPassword))
                    {
                        throw new ArgumentException(
                            "Password must be at least 8 characters long, " +
                            "contain at least one uppercase letter, one lowercase letter, " +
                            "one number and one special character.");
                    }

                    user.PasswordHash = new PasswordHasher<User>()
                        .HashPassword(user, request.NewPassword);
                }
            }

            await _userRepository.UpdateAsync(user);

            return new UserInfoResponse
            {
                Login = user.Login,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                JoiningDate = user.JoiningDate
            };
        }
    }
}