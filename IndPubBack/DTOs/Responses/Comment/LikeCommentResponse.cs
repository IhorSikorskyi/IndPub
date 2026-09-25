namespace IndPubBack.DTOs.Responses.Comment;

public record LikeCommentResponse(Guid CommentId, bool IsLiked);