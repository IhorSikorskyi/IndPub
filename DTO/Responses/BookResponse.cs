using IndPubBack.Models;

namespace IndPubBack.DTO.Responses;

public class BookResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string Language { get; set; } = null!;
    public Status Status { get; set; }
    public int ChapterCount { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<AuthorResponse> Authors { get; set; } = [];
    public List<ChapterShortResponse> Chapters { get; set; } = [];
}

public class BookShortResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string CoverImageUrl { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string Language { get; set; } = null!;
    public Status Status { get; set; }
    public int ChapterCount { get; set; }
    public string GenreName { get; set; }
    public List<AuthorResponse> Authors { get; set; } = [];
}

public class BookmarkShortResponse
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid ChapterId { get; set; }
    public string ChapterTitle { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? BookCoverImageUrl { get; set; }
}

public class LikeBookResponse // This is for both like and unlike, so we can return the current like status after the operation
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid BookId { get; set; }
    public bool IsLiked { get; set; }
}

public class GenreResponse // Create and Update use the same response
{
    public required string GenreName { get; set; }
    public required string GenreDescription { get; set; }
}

// For Delete use DeleteResponse
// But we can use 204 No Content for successful delete, so we don't need a specific response for that

public class AddTagResponse
{
    public Guid BookId { get; set; }
    public string TagName { get; set; } = null!;
    public bool IsAdded { get; set; }
}