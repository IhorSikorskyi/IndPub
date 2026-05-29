namespace IndPubBack.Hubs.Interfaces;

public interface INotificationClient
{
    Task SendBookUpdate(Guid bookId, string bookTitle, string chapterTitle);
    Task SendNewBookAdded(Guid bookId, string bookTitle, string authorLogins);
}