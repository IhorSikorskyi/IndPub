using IndPubBack.Enums;
using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests.Book;

public record BookUpdateRequest
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public IFormFile? CoverImage { get; init; }

    public DateTime UpdateDate { get; init; } = DateTime.Now;

    public Status Status { get; init; } = Status.Ongoing;

    [MinLength(1)]
    public List<Guid>? AuthorIds { get; init; }

    public List<CreateBookTagRequest>? Tags { get; init; }
}