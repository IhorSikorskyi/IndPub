using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Services.Implementations;

public class NotificationService(INotificationRepository notificationRepository, IAccessValidationService accessValidationService, IEntityValidationService entityValidationService) : INotificationService 
{
    // Templates for notification messages
    // Func's are used to allow dynamic insertion of chapter and book names into the messages
    // Can be replaced with a more sophisticated templating system if needed in the future
    // But uses here for demonstration purposes

    private static readonly Func<string, string, string> NewChapterTemplate = (chapter, book) =>
        $"New chapter '{chapter}' added in book '{book}'.";

    private static readonly Func<string, string, string> NewAuthorTemplate = (author, book) =>
        $"New author '{author}' published new book '{book}'.";

    private static readonly string Exception = "Must contain a value";

    public async Task<IEnumerable<NotificationResponse>> GetNotificationsByUserIdAsync(Guid userId, ListNotificationRequest request)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        var notifications = await notificationRepository.GetNotificationsByUserIdAsync(userId, request.Cursor, request.PageSize, request.Type);

        return notifications.Select(MapToNotificationResponse);
    }

    public async Task<NotificationResponse> CreateNotificationAsync(BookNotificationRequest request,
        Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            ChapterId = request.ChapterId,
            Type = NotificationType.NewChapter,
            Message = NewChapterTemplate(request.ChapterName ?? throw new NotFoundException(Exception), request.Title)
        };

        await notificationRepository.AddAsync(notification);

        return MapToNotificationResponse(notification);
    }

    public async Task<NotificationResponse> CreateNotificationAsync(AuthorNotificationRequest request,
        Guid userId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AuthorId = request.AuthorId,
            BookId = request.BookNotification.BookId,
            Type = NotificationType.NewBook,
            Message = NewAuthorTemplate(request.AuthorName ?? throw new NotFoundException(Exception), request.BookNotification.Title)
        };

        await notificationRepository.AddAsync(notification);

        return MapToNotificationResponse(notification);
    }

    public async Task DeleteNotificationAsync(Guid userId, Guid notificationId)
    {
        await entityValidationService.EnsureUserExistsAsync(userId);
        await accessValidationService.EnsureUserIsNotificationOwnerOrModeratorAsync(userId, notificationId);

        await notificationRepository.DeleteAsync(notificationId);
    }

    private static NotificationResponse MapToNotificationResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            Author = notification.Author?.Login,
            BookTitle = notification.Book.Title,
            ChapterTitle = notification.Chapter?.Title,
            Type = notification.Type,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt
        };
    }
}