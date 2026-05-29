using IndPubBack.Models;

namespace IndPubBack.DTO.Responses;

public class LibraryEntryResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int ChapterCount { get; set; }
    public LibraryBookStatus Status { get; set; }
}