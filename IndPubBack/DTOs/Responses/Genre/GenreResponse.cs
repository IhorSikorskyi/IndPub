namespace IndPubBack.DTOs.Responses.Genre;

public record GenreResponse
{
    public required string GenreName { get; init; }
    public required string GenreDescription { get; init; }
}