namespace IndPubBack.DTOs.Responses;

public record CommentResponse // Update and Create use the same response
{
    // UserId from ClaimsPrincipal, so we don't need it here
}

public record CommentShortResponse
{
    public Guid CommentId { get; init; }
    public Guid ChapterId { get; init; }
    public int ChapterNumber { get; init; }
    public string? ChapterTitle { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record LikeCommentResponse(Guid CommentId, bool IsLiked);