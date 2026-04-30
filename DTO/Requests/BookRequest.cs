using System.ComponentModel.DataAnnotations;
using IndPubBack.Models;

namespace IndPubBack.DTO.Requests;

public class BookCreateRequest
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverImage { get; set; }

    public DateTime PublishedDate { get; set; }

    public required string Language { get; set; }

    public required Status Status { get; set; } = Status.Ongoing;

    public required Guid GenreId { get; set; }

    [MinLength(1)]
    public required List<Guid> AuthorIds { get; set; }

    [MinLength(1)]
    public required List<ChapterCreateRequest> Chapters { get; set; }

    public List<Guid>? TagIds { get; set; }
}

public class BookUpdateRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public DateTime UpdateDate { get; set; }

    public string? Language { get; set; }

    public Status? Status { get; set; }

    public Guid? GenreId { get; set; }

    [MinLength(1)]
    public List<Guid>? AuthorIds { get; set; }

    [MinLength(1)]
    public List<ChapterCreateRequest>? Chapters { get; set; }

    public List<Guid>? TagIds { get; set; }
}

public class BookDeleteRequest
{
    public required Guid BookId { get; set; }
}

public class ChapterCreateRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}