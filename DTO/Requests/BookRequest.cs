using IndPubBack.Models;
using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTO.Requests;

public class BookCreateRequest
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverImage { get; set; }

    public DateTime PublishedDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; } = DateTime.Now;

    public required string Language { get; set; }

    public required Status Status { get; set; } = Status.Ongoing;

    public required Guid GenreId { get; set; }

    public required Guid CategoryId { get; set; }
    public required Guid SubcategoryId { get; set; }

    [MinLength(1)]
    public required List<Guid> AuthorIds { get; set; }

    [MinLength(1)]
    public required List<ChapterCreateWithBookRequest> Chapters { get; set; }

    public List<CreateBookTagRequest>? Tags { get; set; }
}

public class BookUpdateRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverImage { get; set; }

    public DateTime UpdateDate { get; set; } = DateTime.Now;

    public Status Status { get; set; } = Status.Ongoing;

    [MinLength(1)]
    public List<Guid>? AuthorIds { get; set; }

    public List<CreateBookTagRequest>? Tags { get; set; }
}

public class ChapterCreateWithBookRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class CreateBookTagRequest
{
    public required string Name { get; set; }
}