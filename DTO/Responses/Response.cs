namespace IndPubBack.DTO.Responses;

public class PagedResponse<T>
{
    public IList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
}