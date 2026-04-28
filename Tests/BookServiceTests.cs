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
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly IBookService _bookService;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:AccessToken"] = "ThisIsAVerySecretKeyForJWTTokenGenerationWithAtLeast32Characters!",
                    ["AppSettings:Issuer"] = "TestIssuer",
                    ["AppSettings:Audience"] = "TestAudience"
                })
                .Build();
            _bookService = new BookService(_configuration, _bookRepositoryMock.Object);
        }

        #region CreateBookAsync Tests

        [Fact]
        public async Task CreateBookAsync_ValidRequest_AddsBookAndReturnsResponse()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            Book? savedBook = null;

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(request.Title))
                .ReturnsAsync((Book?)null);
            _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    b.Id = Guid.NewGuid();
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
            Assert.Equal(request.CoverImageUrl, savedBook.CoverImageUrl);
            Assert.Equal(request.Language, savedBook.Language);
            Assert.Equal(request.Status, savedBook.Status);
            Assert.Equal(request.GenreId, savedBook.GenreId);
            Assert.Equal(request.Chapters.Count, savedBook.ChapterCount);
            Assert.Equal(request.Chapters.Count, savedBook.Chapters.Count);
            Assert.Equal(request.AuthorIds.Count, savedBook.BookAuthors.Count);
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            var existingBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                PublishedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                ChapterCount = 1,
                Language = "en",
                Status = Status.Ongoing,
                GenreId = Guid.NewGuid()
            };

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(request.Title))
                .ReturnsAsync(existingBook);

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
            var existingBook = CreateExistingBook(bookId, "Old Title");
            var request = new BookUpdateRequest
            {
                Title = "Updated Title",
                Description = "Updated description",
                CoverImageUrl = "https://example.com/new-cover.jpg",
                Language = "uk",
                Status = Status.Completed,
                GenreId = Guid.NewGuid(),
                AuthorIds = [Guid.NewGuid(), Guid.NewGuid()],
                Chapters =
                [
                    new ChapterCreateRequest { Title = "New Chapter 1", Content = "Content 1" },
                    new ChapterCreateRequest { Title = "New Chapter 2", Content = "Content 2" }
                ],
                TagIds = [Guid.NewGuid(), Guid.NewGuid()]
            };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(request.Title!))
                .ReturnsAsync((Book?)null);
            _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.UpdateBookAsync(request, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.BookId);
            Assert.Equal(request.Title, result.Title);
            Assert.True(result.UpdatedDate >= existingBook.PublishedDate);
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Updated Title" };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.UpdateBookAsync(request, Guid.NewGuid()));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Taken Title" };
            var existingBook = CreateExistingBook(bookId, "Current Title");
            var anotherBook = CreateExistingBook(Guid.NewGuid(), "Taken Title");

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(request.Title!))
                .ReturnsAsync(anotherBook);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _bookService.UpdateBookAsync(request, Guid.NewGuid()));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        #endregion

        #region DeleteBookAsync Tests

        [Fact]
        public async Task DeleteBookAsync_ValidRequest_DeletesBookAndReturnsResponse()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BookDeleteRequest { BookId = bookId };
            var existingBook = CreateExistingBook(bookId, "Book to delete");

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.DeleteAsync(bookId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.DeleteBookAsync(request, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.BookId);
            Assert.True(result.IsDeleted);
            _bookRepositoryMock.Verify(r => r.DeleteAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BookDeleteRequest { BookId = bookId };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.DeleteBookAsync(request, Guid.NewGuid()));
            _bookRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        private static BookCreateRequest CreateValidBookCreateRequest()
        {
            return new BookCreateRequest
            {
                Title = "Test Book",
                Description = "Test Description",
                CoverImageUrl = "https://example.com/cover.jpg",
                Language = "en",
                Status = Status.Ongoing,
                GenreId = Guid.NewGuid(),
                AuthorIds = [Guid.NewGuid(), Guid.NewGuid()],
                Chapters =
                [
                    new ChapterCreateRequest { Title = "Chapter 1", Content = "Content 1" },
                    new ChapterCreateRequest { Title = "Chapter 2", Content = "Content 2" }
                ],
                TagIds = [Guid.NewGuid(), Guid.NewGuid()]
            };
        }

        private static Book CreateExistingBook(Guid id, string title)
        {
            return new Book
            {
                Id = id,
                Title = title,
                Description = "Existing description",
                CoverImageUrl = "https://example.com/cover.jpg",
                PublishedDate = DateTime.UtcNow.AddDays(-7),
                UpdatedDate = DateTime.UtcNow.AddDays(-1),
                ChapterCount = 1,
                Language = "en",
                Status = Status.Ongoing,
                GenreId = Guid.NewGuid(),
                Chapters =
                [
                    new Chapter
                    {
                        Title = "Existing Chapter",
                        Content = "Existing Content"
                    }
                ],
                BookAuthors =
                [
                    new BookAuthor { UserId = Guid.NewGuid() }
                ]
            };
        }
    }
}