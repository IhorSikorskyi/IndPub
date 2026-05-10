namespace IndPubBack.DTO.Responses;

public class PagedResponse<T>
{
    public IList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
}

public class DeleteResponse
{
    // Use a simple message to indicate success or failure of the delete operation
    // ClaimsPrincipal will be used to get the user id, so we don't need it here
    public bool IsDeleted { get; set; }
}