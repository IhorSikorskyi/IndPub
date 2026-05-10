namespace IndPubBack.DTO.Responses;

public class CommentResponse // Update and Create use the same response
{
    // UserId from ClaimsPrincipal, so we don't need it here
}

public class CommentShortResponse
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class LikeCommentResponse // This is used for both like and unlike, as the client can determine the action based on the IsLiked property
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid CommentId { get; set; }
    public bool IsLiked { get; set; }
}