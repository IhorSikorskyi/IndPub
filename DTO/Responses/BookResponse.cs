namespace IndPubBack.DTO.Responses;

public class BookCreateResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class BookUpdateResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
}

public class BookDeleteResponse
{
    public Guid BookId { get; set; }
    public bool IsDeleted { get; set; }
}