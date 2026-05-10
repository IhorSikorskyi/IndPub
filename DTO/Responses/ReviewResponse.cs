namespace IndPubBack.DTO.Responses;

public class ReviewResponse // Create, Update and Read use the same response
{
    // UserId from ClaimsPrincipal, so we don't need it here
}

public class ReviewShortResponse
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}