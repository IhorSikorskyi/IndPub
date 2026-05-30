using IndPubBack.DTO.Requests;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Implementations;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IChapterRepository> _chapterRepositoryMock;
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        private readonly IPasswordValidationService _passwordValidationService;
        private readonly IEntityValidationService _entityValidationService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _chapterRepositoryMock = new Mock<IChapterRepository>();
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordValidationService = new PasswordValidationService();
            _entityValidationService = new EntityValidationService(
                _userRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _chapterRepositoryMock.Object,
                _reviewRepositoryMock.Object);

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:AccessToken"] = "ThisIsAVerySecretKeyForJWTTokenGenerationWithAtLeast32Characters!",
                    ["AppSettings:Issuer"] = "TestIssuer",
                    ["AppSettings:Audience"] = "TestAudience"
                })
                .Build();

            _userRepositoryMock
                .Setup(r => r.GetUserRoleAsync(It.IsAny<Guid>()))
                .ReturnsAsync("User");

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _authService = new AuthService(
                _configuration,
                _userRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordValidationService,
                _entityValidationService);
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

            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result.response);
            Assert.NotNull(result.response.AccessToken);
            Assert.NotNull(result.refreshToken);
            Assert.True(result.refreshTokenExpiry > DateTime.UtcNow);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _refreshTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Theory]
        [InlineData("ab", "test@example.com", "Test@1234", "Test@1234")]       // Short login
        [InlineData("testuser", "invalidemail", "Test@1234", "Test@1234")]      // Invalid email
        [InlineData("", "test@example.com", "Test@1234", "Test@1234")]          // Empty login
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
            await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(request));
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

            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(request.Login))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(request.Email))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync(request));
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

            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(request));
        }

        [Theory]
        [InlineData("weak")]              // Too short
        [InlineData("alllowercase1!")]    // No uppercase
        [InlineData("ALLUPPERCASE1!")]    // No lowercase
        [InlineData("NoNumbers!")]        // No digits
        [InlineData("NoSpecial123")]      // No special chars
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

            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(request));
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
                    .HashPassword(null!, "Test@1234")
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(request.LoginOrEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result.response);
            Assert.NotNull(result.response.AccessToken);
            Assert.NotNull(result.refreshToken);
            Assert.True(result.refreshTokenExpiry > DateTime.UtcNow);
            _refreshTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
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
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.LoginAsync(request));
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
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.LoginAsync(request));
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {
            // Arrange
            var rawToken = "validrawtoken";
            var hashedToken = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(rawToken)));

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshTokenHash = hashedToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(15)
            };

            _refreshTokenRepositoryMock.Setup(r => r.GetByHashAsync(hashedToken))
                .ReturnsAsync(refreshToken);
            _refreshTokenRepositoryMock.Setup(r => r.RevokeTokenForUserAsync(refreshToken.Id))
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(rawToken);

            // Assert
            _refreshTokenRepositoryMock.Verify(r => r.RevokeTokenForUserAsync(refreshToken.Id), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_RevokedToken_ThrowsSecurityException()
        {
            // Arrange
            var rawToken = "revokedtoken";
            var hashedToken = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(rawToken)));

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshTokenHash = hashedToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(15),
                RevokedAt = DateTime.UtcNow.AddHours(-1)
            };

            _refreshTokenRepositoryMock.Setup(r => r.GetByHashAsync(hashedToken))
                .ReturnsAsync(refreshToken);
            _refreshTokenRepositoryMock.Setup(r => r.RevokeAllTokensForUserAsync(refreshToken.UserId))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(() => _authService.LogoutAsync(rawToken));
            _refreshTokenRepositoryMock.Verify(r => r.RevokeAllTokensForUserAsync(refreshToken.UserId), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ExpiredToken_ThrowsValidationException()
        {
            // Arrange
            var rawToken = "expiredtoken";
            var hashedToken = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(rawToken)));

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshTokenHash = hashedToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepositoryMock.Setup(r => r.GetByHashAsync(hashedToken))
                .ReturnsAsync(refreshToken);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.LogoutAsync(rawToken));
        }

        #endregion
    }
}