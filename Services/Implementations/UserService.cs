using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IndPubBack.Services.Implementations
{
    public class UserService(IConfiguration configuration, IUserRepository userRepository, IBlobService blobService) : IUserService
    {
        private static readonly string _check = "Invalid access token.";

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Length < 3
                                                         || string.IsNullOrWhiteSpace(request.Email) ||
                                                         !request.Email.Contains('@'))
            {
                throw new ValidationException("Invalid login or email.");
            }

            var existingUser = await userRepository.GetByLoginAsync(request.Login)
                               ?? await userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new ConflictException("User with the same username or email already exists.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                throw new ValidationException("Passwords do not match.");
            }

            EnsurePasswordComplex(request.Password);

            var user = new User
            {
                Login = request.Login,
                Email = request.Email,
                PasswordHash = null!, // Will be set after hashing
                RefreshToken = null! // Will be set after generation
            };

            user.PasswordHash = HashPassword(user, request.Password);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await userRepository.AddAsync(user);

            return await CreateAccessTokenResponseAsync(user);
        }

        public async Task<UserResponse> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByLoginAsync(request.LoginOrEmail)
                       ?? await userRepository.GetByEmailAsync(request.LoginOrEmail);

            if (user == null)
            {
                throw new InvalidCredentialsException("Invalid user or password.");
            }

            var verificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidCredentialsException("Invalid user or password.");
            }

            if (!ValidateRefreshToken(user))
            {
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await userRepository.UpdateAsync(user);
            }

            return await CreateAccessTokenResponseAsync(user);
        }

        public async Task<bool> LogoutAsync(string accessToken, string refreshToken)
        {
            var userId = GetUserIdFromClaims(accessToken, configuration);
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedException(_check);
            }

            if (!ValidateRefreshToken(user, refreshToken) || !ValidateRefreshToken(user))
            {
                throw new UnauthorizedException("Invalid refresh token. Please log in again.");
            }

            var tokenExpiredSuccessfully = await userRepository.ExpireRefreshTokenAsync(user.Id);

            if (!tokenExpiredSuccessfully)
            {
                throw new UnauthorizedException("Failed to expire refresh token. User may have been deleted.");
            }

            return true;
        }

        public async Task<UserResponse> UpdateAccessTokenAsync(string accessToken, string refreshToken)
        {
            var userId = GetUserIdFromClaims(accessToken, configuration);
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedException(_check);
            }

            if (!ValidateRefreshToken(user, refreshToken) || !ValidateRefreshToken(user))
            {
                throw new UnauthorizedException("Invalid refresh token. Please log in again.");
            }

            return await CreateAccessTokenResponseAsync(user);
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var userId = GetUserIdFromClaims(accessToken, configuration);

            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

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
            var userId = GetUserIdFromClaims(accessToken, configuration);
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedException(_check);
            }

            if (!string.IsNullOrWhiteSpace(request.Bio))
            {
                user.Bio = request.Bio;
            }

            if (request.ProfilePicture is not null)
            {
                if (!AvatarImageValidation(request.ProfilePicture))
                {
                    throw new ValidationException("Invalid image");
                }

                string folder = configuration["AzureStorage:ProfilePicturesFolder"]!;

                string avatarUrl = await blobService.UploadBlobAsync(folder, request.ProfilePicture, user.Id);

                user.ProfilePictureUrl = avatarUrl;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await userRepository.GetByEmailAsync(request.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                {
                    throw new ConflictException("Email already in use.");
                }

                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                var verificationResult = new PasswordHasher<User>()
                    .VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    throw new InvalidCredentialsException("Invalid password.");
                }

                if (!string.IsNullOrWhiteSpace(request.NewPassword) &&
                    !string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                {
                    if (request.NewPassword != request.ConfirmNewPassword)
                        throw new ValidationException("New passwords do not match.");

                    EnsurePasswordComplex(request.NewPassword);
                    user.PasswordHash = new PasswordHasher<User>()
                        .HashPassword(user, request.NewPassword);
                }
            }

            await userRepository.UpdateAsync(user);

            return new UserInfoResponse
            {
                Login = user.Login,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                JoiningDate = user.JoiningDate
            };
        }

        private static Guid GetUserIdFromClaims(string accessToken, IConfiguration configuration)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration.GetValue<string>("AppSettings:Issuer"),
                ValidateAudience = true,
                ValidAudience = configuration.GetValue<string>("AppSettings:Audience"),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:AccessToken")!)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            try
            {
                handler.ValidateToken(accessToken, validationParameters, out var validated);
                var jwt = (JwtSecurityToken)validated;
                var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    throw new SecurityTokenException("User ID claim missing");
                return userId;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedException(_check);
            }
            catch (SecurityTokenException)
            {
                throw new UnauthorizedException(_check);
            }
        }

        private async Task<UserResponse> CreateAccessTokenResponseAsync(User user)
        {

            return new UserResponse
            {
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry,
                AccessToken = await CreateTokenAsync(user)
            };
        }

        private async Task<string> CreateTokenAsync(User user)
        {
            var role = await userRepository.GetUserRoleAsync(user.Id);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:AccessToken")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var accessToken = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
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

        private static void EnsurePasswordComplex(string password)
        {
            if (IsPasswordComplex(password))
            {
                return;
            }

            throw new ValidationException(
                "Password must be at least 8 characters long, " +
                "contain at least one uppercase letter, one lowercase letter, " +
                "one number and one special character.");
        }

        private static bool IsPasswordComplex(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;

                if (hasUpper && hasLower && hasDigit && hasSpecial)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ValidateRefreshToken(User user, string refreshToken)
        {
            if (user == null || user.RefreshToken != refreshToken)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateRefreshToken(User user)
        {
            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        private static bool AvatarImageValidation(IFormFile image)
        {
            if (image.Length == 0)
            {
                return false;
            }

            const long maxFileSize = 2 * 1024 * 1024;
            if (image.Length > maxFileSize)
            {
                return false;
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png"
            };

            var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg", "image/png", "image/jpg"
            };

            var extension = Path.GetExtension(image.FileName);

            return !string.IsNullOrWhiteSpace(extension)
                   && allowedExtensions.Contains(extension)
                   && !string.IsNullOrWhiteSpace(image.ContentType)
                   && allowedContentTypes.Contains(image.ContentType);
        }
    }
}