using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Entities;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            IPasswordValidationService passwordValidationService = new PasswordValidationService();

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _jwtTokenGeneratorMock
                .Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns(new AccessTokenResponse("fake-access-token"));

            _refreshTokenServiceMock
                .Setup(r => r.CreateRefreshTokenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(("fake-refresh-token-hash", DateTime.UtcNow.AddDays(15)));

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _jwtTokenGeneratorMock.Object,
                _refreshTokenServiceMock.Object,
                passwordValidationService);
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
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiry > DateTime.UtcNow);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ExistingUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "existinguser",
                Email = "test@example.com",
                Password = "Test@1234",
                ConfirmPassword = "Test@1234"
            };

            _userRepositoryMock.Setup(r => r.IsExistByLoginOrEmailAsync(request.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_PasswordMismatch_ThrowsInvalidOperationException()
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
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(request));
        }

        [Theory]
        [InlineData("weak")]
        [InlineData("alllowercase1!")]
        [InlineData("ALLUPPERCASE1!")]
        [InlineData("NoNumbers!")]
        [InlineData("NoSpecial123")]
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
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Test@1234")
            };

            _userRepositoryMock.Setup(r => r.GetByLoginAsync(request.LoginOrEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiry > DateTime.UtcNow);
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
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Test@1234")
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
            var tokenId = Guid.NewGuid();
            var refreshToken = new RefreshToken
            {
                Id = tokenId,
                UserId = Guid.NewGuid(),
                RefreshTokenHash = "hashed",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(15)
            };

            _refreshTokenServiceMock.Setup(r => r.ValidateUserRefreshTokenAsync(rawToken))
                .ReturnsAsync(refreshToken);
            _refreshTokenServiceMock.Setup(r => r.RevokeRefreshTokenAsync(tokenId))
                .ReturnsAsync(true);

            // Act
            await _authService.LogoutAsync(rawToken);

            // Assert
            _refreshTokenServiceMock.Verify(r => r.RevokeRefreshTokenAsync(tokenId), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_RevokedToken_ThrowsSecurityException()
        {
            // Arrange
            var rawToken = "revokedtoken";

            _refreshTokenServiceMock.Setup(r => r.ValidateUserRefreshTokenAsync(rawToken))
                .ThrowsAsync(new SecurityException("Token has been revoked."));

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(() => _authService.LogoutAsync(rawToken));
        }

        [Fact]
        public async Task LogoutAsync_ExpiredToken_ThrowsValidationException()
        {
            // Arrange
            var rawToken = "expiredtoken";

            _refreshTokenServiceMock.Setup(r => r.ValidateUserRefreshTokenAsync(rawToken))
                .ThrowsAsync(new ValidationException("Refresh token has expired."));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.LogoutAsync(rawToken));
        }

        #endregion
    }
}