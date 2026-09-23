namespace IndPubBack.DTOs.Responses;

public record ReviewResponse
{
    public Guid Id { get; init; }
    public Guid? BookId { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public double? Rating { get; init; }
    public string? Text { get; init; }
    public DateTime CreatedAt { get; init; }
}