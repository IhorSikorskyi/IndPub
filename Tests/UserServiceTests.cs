using Xunit;
using Moq;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Models;
using IndPubBack.DTO.Requests;
using IndPubBack.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:AccessToken"] = "ThisIsAVerySecretKeyForJWTTokenGenerationWithAtLeast32Characters!",
                    ["AppSettings:Issuer"] = "TestIssuer",
                    ["AppSettings:Audience"] = "TestAudience"
                })
                .Build();

            _userService = new UserService(_configuration, _userRepositoryMock.Object);
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_ValidRequest_ReturnsUserResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "testuser",
                Email = "test@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234"
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(It.IsAny<string>()))
                // .ReturnsAsync((User?)null);
                .Returns(Task.FromResult<User?>(null));
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Theory]
        [InlineData("ab", "test@example.com", "Test@1234", "Test@1234")] // Short login
        [InlineData("testuser", "invalidemail", "Test@1234", "Test@1234")] // Invalid email
        [InlineData("", "test@example.com", "Test@1234", "Test@1234")] // Empty login
        public async Task RegisterAsync_InvalidInput_ThrowsValidationException(
            string login, string email, string password, string confirmPassword)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = login,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _userService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ExistingUser_ThrowsConflictException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "existinguser",
                Email = "test@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234"
            };

            var existingUser = new User { Id = Guid.NewGuid(), Login = "existinguser", Email = "existinguser@example.com" };
            _userRepositoryMock.Setup(r => r.GetByLoginAsync(request.Login))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _userService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_PasswordMismatch_ThrowsValidationException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "testuser",
                Email = "test@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Different@1234"
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _userService.RegisterAsync(request));
        }

        [Theory]
        [InlineData("weak")] // Too short
        [InlineData("alllowercase1!")] // No uppercase
        [InlineData("ALLUPPERCASE1!")] // No lowercase
        [InlineData("NoNumbers!")] // No digits
        [InlineData("NoSpecial123")] // No special chars
        public async Task RegisterAsync_WeakPassword_ThrowsValidationException(string password)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "testuser",
                Email = "test@example.com",
                Password = password,
                ConfirmPassword = password
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _userService.RegisterAsync(request));
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUserResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                LoginOrEmail = "testuser",
                Password = "Test@1234"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = "testuser",
                Email = "testuser@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>()
                    .HashPassword(null!, "Test@1234"),
                RefreshToken = "validtoken",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(request.LoginOrEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_InvalidUser_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var request = new LoginRequest
            {
                LoginOrEmail = "nonexistent",
                Password = "Test@1234"
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var request = new LoginRequest
            {
                LoginOrEmail = "testuser",
                Password = "WrongPassword@123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = "testuser",
                Email = "testuser@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>()
                    .HashPassword(null!, "Test@1234")
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(request.LoginOrEmail))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.LoginAsync(request));
        }

        #endregion

        #region GetUserInfoAsync Tests

        [Fact]
        public async Task GetUserInfoAsync_ValidToken_ReturnsUserInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Login = "testuser",
                Email = "test@example.com",
                Bio = "Test bio",
                ProfilePictureUrl = "https://example.com/pic.jpg",
                JoiningDate = DateTime.UtcNow.AddDays(-30)
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var accessToken = GenerateValidToken(userId);

            // Act
            var result = await _userService.GetUserInfoAsync(accessToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Login, result.Login);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Bio, result.Bio);
        }

        [Fact]
        public async Task GetUserInfoAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var accessToken = GenerateValidToken(userId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserInfoAsync(accessToken));
        }

        #endregion

        #region UpdateUserInfoAsync Tests

        [Fact]
        public async Task UpdateUserInfoAsync_ValidRequest_UpdatesAndReturnsUserInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Login = "testuser",
                Email = "old@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>()
                    .HashPassword(null!, "Test@1234")
            };

            var request = new UpdateProfileRequest
            {
                Email = "new@example.com",
                Bio = "Updated bio"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("new@example.com"))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var accessToken = GenerateValidToken(userId);

            // Act
            var result = await _userService.UpdateUserInfoAsync(accessToken, request);

            // Assert
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal("Updated bio", result.Bio);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoAsync_EmailAlreadyExists_ThrowsConflictException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Login = "testuser", Email = "old@example.com" };
            var otherUser = new User { Id = Guid.NewGuid(), Login = "existinguser", Email = "existing@example.com" };

            var request = new UpdateProfileRequest { Email = "existing@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("existing@example.com"))
                .ReturnsAsync(otherUser);

            var accessToken = GenerateValidToken(userId);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => 
                _userService.UpdateUserInfoAsync(accessToken, request));
        }

        #endregion

        private string GenerateValidToken(Guid userId)
        {
            var user = new User
            {
                Id = userId,
                Login = "testuser",
                Email = "test@example.com"
            };

            var service = new UserService(_configuration, _userRepositoryMock.Object);
            var response = typeof(UserService)
                .GetMethod("CreateToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(service, new object[] { user });

            return response?.ToString() ?? string.Empty;
        }
    }
}