using IndPubBack.Enums;
using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTOs.Requests.Search;

public record BookSearchRequest
{
    public string? Title { get; init; }
    public List<string>? AuthorName { get; init; }
    public DateTime? PublishDateFrom { get; init; }
    public DateTime? PublishDateTo { get; init; }
    public DateTime? UpdatedDateFrom { get; init; }
    public DateTime? UpdatedDateTo { get; init; }
    public LanguageCode? Language { get; init; }
    public Status? Status { get; init; }
    public string? GenreName { get; init; }
    public string? CategoryName { get; init; }
    public string? SubcategoryName { get; init; }
    public double? MinRating { get; init; }
    public int? MinLikes { get; init; }
    public int? MinChapters { get; init; }
    public List<string>? BookTagName { get; init; }

    public BookSortingBy SortingBy { get; init; } = BookSortingBy.Rating;
    public bool Descending { get; init; } = true;

    public string? Cursor { get; init; }

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
}