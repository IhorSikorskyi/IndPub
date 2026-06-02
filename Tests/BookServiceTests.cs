using IndPubBack.DTO.Requests;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Implementations;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;

namespace Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IChapterRepository> _chapterRepositoryMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly IConfiguration _configuration;
        private readonly BookService _bookService;
        private readonly Mock<IImageValidationService> _imageValidationServiceMock;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessValidationService _accessValidationService;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _chapterRepositoryMock = new Mock<IChapterRepository>();
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _blobServiceMock = new Mock<IBlobService>();
            _imageValidationServiceMock = new Mock<IImageValidationService>();
            _imageValidationServiceMock.Setup(r => r.ValidateImage(It.IsAny<IFormFile>(), It.IsAny<long>()))
                .Returns(true);
            _entityValidationService = new EntityValidationService(
                _userRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _chapterRepositoryMock.Object,
                _reviewRepositoryMock.Object);
            _accessValidationService = new AccessValidationService(
                _userRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _reviewRepositoryMock.Object,
                _notificationRepositoryMock.Object);
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:AccessToken"] = "ThisIsAVerySecretKeyForJWTTokenGenerationWithAtLeast32Characters!",
                    ["AppSettings:Issuer"] = "TestIssuer",
                    ["AppSettings:Audience"] = "TestAudience"
                })
                .Build();
            _bookService = new BookService(
                _configuration,
                _bookRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _imageValidationServiceMock.Object,
                _entityValidationService,
                _accessValidationService,
                _blobServiceMock.Object);
        }

        #region CreateBookAsync Tests

        [Fact]
        public async Task CreateBookAsync_ValidRequest_AddsBookAndReturnsResponse()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            Book? savedBook = null;

            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.GetExistingIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(request.AuthorIds);
            _userRepositoryMock.Setup(r => r.IsExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _tagRepositoryMock.Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Tag?)null);
            _tagRepositoryMock.Setup(r => r.AddAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) => new Tag { Id = Guid.NewGuid(), Name = name });
            _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    b.Id = Guid.NewGuid();
                    b.Genre = new Genre { Name = "TestGenre", Description = "Test" };
                    b.Category = new Category { Name = "TestCategory", Description = "Test" };
                    b.Subcategory = new Subcategory { Name = "TestSubcategory", Description = "Test" };
                    foreach (var ba in b.BookAuthors)
                        ba.User = new User { Login = "testauthor", Email = "test@test.com", PasswordHash = "hash" };
                    foreach (var bt in b.BookTags)
                        bt.Tag = new Tag { Name = "TestTag" };
                    savedBook = b;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.CreateBookAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.BookId);
            Assert.Equal(request.Title, result.Title);
            Assert.NotNull(savedBook);
            Assert.Equal(request.Title, savedBook!.Title);
            Assert.Equal(request.Description, savedBook.Description);
            Assert.Equal(request.Language, savedBook.Language);
            Assert.Equal(request.Status, savedBook.Status);
            Assert.Equal(request.GenreId, savedBook.GenreId);
            Assert.Equal(request.Chapters.Count, savedBook.Chapters.Count);
            Assert.Equal(request.AuthorIds.Count, savedBook.BookAuthors.Count);
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();

            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _bookService.CreateBookAsync(request));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyTitle_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            request.Title = "   ";

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyAuthors_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            request.AuthorIds = [];

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyChapters_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            request.Chapters = [];

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        #endregion

        #region UpdateBookAsync Tests

        [Fact]
        public async Task UpdateBookAsync_ValidRequest_UpdatesBookAndReturnsResponse()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingBook = CreateExistingBook(bookId, "Old Title", userId);

            var request = new BookUpdateRequest
            {
                Title = "Updated Title",
                Description = "Updated description",
                Status = Status.Completed,
                AuthorIds = [Guid.NewGuid(), Guid.NewGuid()],
                Tags =
                [
                    new CreateBookTagRequest { Name = "Fantasy" },
                    new CreateBookTagRequest { Name = "Adventure" }
                ]
            };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(false);
            _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.IsExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _tagRepositoryMock.Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Tag?)null);
            _tagRepositoryMock.Setup(r => r.AddAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) => new Tag { Id = Guid.NewGuid(), Name = name });
            _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    foreach (var ba in b.BookAuthors)
                        ba.User ??= new User { Login = "testauthor", Email = "test@test.com", PasswordHash = "hash" };
                    foreach (var bt in b.BookTags)
                        bt.Tag ??= new Tag { Name = "TestTag" };
                })
                .Returns(Task.CompletedTask);


            // Act
            var result = await _bookService.UpdateBookAsync(request, bookId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.BookId);
            Assert.Equal(request.Title, result.Title);
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Updated Title" };

            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.UpdateBookAsync(request, bookId, userId));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Taken Title" };
            var existingBook = CreateExistingBook(bookId, "Current Title", userId);

            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _bookService.UpdateBookAsync(request, bookId, userId));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        #endregion

        #region DeleteBookAsync Tests

        [Fact]
        public async Task DeleteBookAsync_ValidRequest_DeletesBookAndReturnsTrue()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingBook = CreateExistingBook(bookId, "Book to delete", userId);

            _bookRepositoryMock.Setup(r => r.IsExistAsync(bookId))
                .ReturnsAsync(true);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.DeleteAsync(bookId))
                .Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");

            // Act
            var result = await _bookService.DeleteBookAsync(bookId, userId);

            // Assert
            Assert.True(result);
            _bookRepositoryMock.Verify(r => r.DeleteAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _bookRepositoryMock.Setup(r => r.IsExistAsync(bookId))
                .ReturnsAsync(false);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.DeleteBookAsync(bookId, userId));
            _bookRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteBookAsync_UserNotAuthor_ThrowsForbiddenException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _bookRepositoryMock.Setup(r => r.IsExistAsync(bookId))
                .ReturnsAsync(true);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.GetUserRoleAsync(userId))
                .ReturnsAsync("User");

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _bookService.DeleteBookAsync(bookId, userId));
            _bookRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        private static BookCreateRequest CreateValidBookCreateRequest()
        {
            return new BookCreateRequest
            {
                Title = "Test Book",
                Description = "Test Description",
                CoverImage = CreateFormFile(),
                Language = LanguageCode.En,
                Status = Status.Ongoing,
                GenreId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                SubcategoryId = Guid.NewGuid(),
                AuthorIds = [Guid.NewGuid(), Guid.NewGuid()],
                Chapters =
                [
                    new ChapterCreateWithBookRequest { Title = "Chapter 1", Content = "Content 1" },
                    new ChapterCreateWithBookRequest { Title = "Chapter 2", Content = "Content 2" }
                ],
                Tags =
                [
                    new CreateBookTagRequest { Name = "Fantasy" },
                    new CreateBookTagRequest { Name = "Adventure" }
                ]
            };
        }

        private static FormFile CreateFormFile()
        {
            var content = Encoding.UTF8.GetBytes("fake image content");
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "CoverImage", "cover.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        private static Book CreateExistingBook(Guid id, string title, Guid? authorId = null)
        {
            return new Book
            {
                Id = id,
                Title = title,
                Description = "Existing description",
                CoverImageUrl = "https://example.com/cover.jpg",
                PublishedDate = DateTime.UtcNow.AddDays(-7),
                UpdatedDate = DateTime.UtcNow.AddDays(-1),
                Language = LanguageCode.En,
                Status = Status.Ongoing,
                GenreId = Guid.NewGuid(),
                Genre = new Genre { Name = "TestGenre", Description = "Test" },
                Category = new Category { Name = "TestCategory", Description = "Test" },
                Subcategory = new Subcategory { Name = "TestSubcategory", Description = "Test" },
                Chapters =
                [
                    new Chapter
                    {
                        Title = "Existing Chapter",
                        Content = "Existing Content",
                        ChapterNumber = 1
                    }
                ],
                BookAuthors =
                [
                    new BookAuthor
                    {
                        UserId = authorId ?? Guid.NewGuid(),
                        User = new User
                        {
                            Login = "testauthor",
                            Email = "author@test.com",
                            PasswordHash = "hash"
                        }
                    }
                ]
            };
        }
    }
}