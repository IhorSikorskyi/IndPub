using IndPubBack.Enums;
using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests.Book;

public record BookCreateRequest
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public IFormFile? CoverImage { get; init; }

    public DateTime PublishedDate { get; init; } = DateTime.Now;
    public DateTime? UpdateDate { get; init; } = DateTime.Now;

    public required LanguageCode Language { get; init; } = LanguageCode.En;

    public required Status Status { get; init; } = Status.Ongoing;

    public required Guid GenreId { get; init; }

    public required Guid CategoryId { get; init; }
    public required Guid SubcategoryId { get; init; }

    [MinLength(1)]
    public required List<Guid> AuthorIds { get; init; }

    [MinLength(1)]
    public required List<ChapterCreateWithBookRequest> Chapters { get; init; }

    public List<CreateBookTagRequest>? Tags { get; init; }
}