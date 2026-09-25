namespace IndPubBack.DTOs.Requests.Book;

public record CreateBookTagRequest
{
    public required string Name { get; init; }
}