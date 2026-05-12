namespace IndPubBack.DTO.Responses;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}