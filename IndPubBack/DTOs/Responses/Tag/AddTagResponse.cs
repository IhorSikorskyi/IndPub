namespace IndPubBack.DTOs.Responses.Tag;

public record AddTagResponse
{
    public Guid BookId { get; init; }
    public required string TagName { get; init; }
    public bool IsAdded { get; init; }
}