using IndPubBack.Enums;

namespace IndPubBack.DTOs.Responses.Library;

public record LibraryEntryResponse
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public DateTime UpdatedDate { get; init; }
    public int ChapterCount { get; init; }
    public LibraryBookStatus Status { get; init; }
}