using IndPubBack.DTOs.Requests.Book;
using IndPubBack.Entities;
using IndPubBack.Enums;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITagService> _tagServiceMock;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tagServiceMock = new Mock<ITagService>();
            var imageServiceMock = new Mock<IImageService>();
            var entityValidationServiceMock = new Mock<IEntityValidationService>();
            var accessValidationServiceMock = new Mock<IAccessValidationService>();

            entityValidationServiceMock
                .Setup(e => e.IsUserExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            imageServiceMock
                .Setup(i => i.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync("https://example.com/cover.jpg");

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            accessValidationServiceMock
                .Setup(a => a.IsUserIsModeratorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _bookService = new BookService(
                _bookRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _tagServiceMock.Object,
                imageServiceMock.Object,
                entityValidationServiceMock.Object,
                accessValidationServiceMock.Object);
        }

        #region CreateBookAsync Tests

        [Fact]
        public async Task CreateBookAsync_ValidRequest_AddsBookAndReturnsResponse()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            var currentUserId = request.AuthorIds[0];
            Book? savedBook = null;

            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(false);

            _tagServiceMock
                .Setup(t => t.GetOrCreateBookTagsAsync(It.IsAny<Guid>(), It.IsAny<List<CreateBookTagRequest>>()))
                .ReturnsAsync((Guid bookId, List<CreateBookTagRequest> tags) =>
                    tags.Select(t => new BookTag { BookId = bookId, TagId = Guid.NewGuid(), Tag = new Tag { Name = t.Name } }).ToList());

            _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    b.Id = Guid.NewGuid();
                    b.Genre = new Genre { Name = "TestGenre", Description = "Test" };
                    b.Category = new Category { Name = "TestCategory", Description = "Test" };
                    b.Subcategory = new Subcategory { Name = "TestSubcategory", Description = "Test" };
                    foreach (var ba in b.BookAuthors)
                        ba.User = new User { Login = "testauthor", Email = "test@test.com", PasswordHash = "hash" };
                    savedBook = b;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.CreateBookAsync(request, currentUserId);

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
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest();
            var currentUserId = request.AuthorIds[0];

            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _bookService.CreateBookAsync(request, currentUserId));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyTitle_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest() with { Title = "   " };
            var currentUserId = request.AuthorIds[0];

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request, currentUserId));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyAuthors_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest() with { AuthorIds = [] };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request, Guid.NewGuid()));
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_EmptyChapters_ThrowsValidationException()
        {
            // Arrange
            var request = CreateValidBookCreateRequest() with { Chapters = [] };
            var currentUserId = request.AuthorIds[0];

            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(false);

            // Act & Assert
            // Примітка: у BookService.CreateBookAsync немає явної перевірки на порожні Chapters —
            // якщо ця перевірка справді потрібна, її треба додати в сервіс (зараз тест може не пройти)
            await Assert.ThrowsAsync<ValidationException>(() => _bookService.CreateBookAsync(request, currentUserId));
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
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);

            _tagServiceMock
                .Setup(t => t.GetOrCreateBookTagsAsync(bookId, It.IsAny<List<CreateBookTagRequest>>()))
                .ReturnsAsync((Guid bId, List<CreateBookTagRequest> tags) =>
                    tags.Select(t => new BookTag { BookId = bId, TagId = Guid.NewGuid(), Tag = new Tag { Name = t.Name } }).ToList());

            _bookRepositoryMock.Setup(r => r.Update(It.IsAny<Book>()))
                .Callback<Book>(b =>
                {
                    foreach (var ba in b.BookAuthors)
                        ba.User ??= new User { Login = "testauthor", Email = "test@test.com", PasswordHash = "hash" };
                });

            // Act
            var result = await _bookService.UpdateBookAsync(request, bookId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.BookId);
            Assert.Equal(request.Title, result.Title);
            _bookRepositoryMock.Verify(r => r.Update(It.IsAny<Book>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Updated Title" };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.UpdateBookAsync(request, bookId, userId));
            _bookRepositoryMock.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_DuplicateTitle_ThrowsConflictException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new BookUpdateRequest { Title = "Taken Title" };
            var existingBook = CreateExistingBook(bookId, "Current Title", userId);

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);
            _bookRepositoryMock.Setup(r => r.HasTitleAsync(request.Title))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _bookService.UpdateBookAsync(request, bookId, userId));
            _bookRepositoryMock.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
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

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(true);

            // Act
            var result = await _bookService.DeleteBookAsync(bookId, userId);

            // Assert
            Assert.True(result);
            _bookRepositoryMock.Verify(r => r.Delete(existingBook), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_BookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookService.DeleteBookAsync(bookId, userId));
            _bookRepositoryMock.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task DeleteBookAsync_UserNotAuthor_ThrowsAccessViolationException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingBook = CreateExistingBook(bookId, "Book", userId);

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.IsUserAuthorAsync(userId, bookId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<AccessViolationException>(() => _bookService.DeleteBookAsync(bookId, userId));
            _bookRepositoryMock.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
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
                CreatedAt = DateTime.UtcNow.AddDays(-7),
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