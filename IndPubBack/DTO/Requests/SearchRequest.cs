using IndPubBack.Models;
using System.ComponentModel.DataAnnotations;

namespace IndPubBack.DTO.Requests;

public class BookSearchRequest
{
    public string? Title { get; set; }
    public List<string>? AuthorName { get; set; }
    public DateTime? PublishDateFrom { get; set; }
    public DateTime? PublishDateTo { get; set; }
    public DateTime? UpdatedDateFrom { get; set; }
    public DateTime? UpdatedDateTo { get; set; }
    public LanguageCode? Language { get; set; }
    public Status? Status { get; set; }
    public string? GenreName { get; set; }
    public string? CategoryName { get; set; }
    public string? SubcategoryName { get; set; }
    public double? MinRating { get; set; }
    public int? MinLikes { get; set; }
    public int? MinChapters { get; set; }
    public List<string>? BookTagName { get; set; }

    public BookSortingBy SortingBy { get; set; } = BookSortingBy.Rating;
    public bool Descending { get; set; } = true;

    public string? Cursor { get; set; }
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public enum BookSortingBy
{
    Title,
    PublishDate,
    UpdatedDate,
    LikesNumber,
    ChaptersNumber,
    Rating
}