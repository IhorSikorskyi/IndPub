using IndPubBack.Models;

namespace IndPubBack.DTO.Responses;

public class UserResponse
{
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}

public class UserInfoResponse
{
    public string Login { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime? JoiningDate { get; set; }
    public int? SubscribersCount { get; set; }

}

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}

public class UserActivitiesResponse
{
    public IList<LibraryEntryResponse>? Library { get; set; } = [];
    public IList<BookShortResponse>? LikedBooks { get; set; } = [];
    public IList<ReviewResponse>? Reviews { get; set; } = [];
    public IList<ReviewResponse>? LikeReview { get; set; } = [];
    public IList<CommentShortResponse>? Comments { get; set; } = [];
    public IList<LikeCommentResponse>? LikeComment { get; set; } = [];
    public IList<SubscriptionShortResponse>? Subscriptions { get; set; } = [];
    public IList<BookmarkShortResponse>? Bookmarks { get; set; } = [];
}