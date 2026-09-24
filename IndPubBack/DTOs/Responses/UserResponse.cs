using IndPubBack.Entities;

namespace IndPubBack.DTOs.Responses;

public record UserResponse
{
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; init; }
    public AccessTokenResponse AccessToken { get; init; } = new (string.Empty);
    public override string ToString() =>
        $"{nameof(UserResponse)} {{ AccessToken = [REDACTED], RefreshToken = [REDACTED], RefreshTokenExpiry = {RefreshTokenExpiry} }}";
}

public record AccessTokenResponse(string AccessToken)
{
    public override string ToString() => $"{nameof(AccessTokenResponse)} {{ AccessToken = [REDACTED] }}";
}

public record UserInfoResponse
{
    public string Login { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Bio { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public DateTime? JoiningDate { get; init; }
    public int? SubscribersCount { get; init; }
}

public record AuthorResponse
{
    public Guid Id { get; init; }
    public required string Login { get; init; }
    public string? ProfilePictureUrl { get; init; }
}

public record UserActivitiesResponse
{
    public IList<LibraryEntryResponse>? Library { get; init; } = [];
    public IList<BookShortResponse>? LikedBooks { get; init; } = [];
    public IList<ReviewResponse>? Reviews { get; init; } = [];
    public IList<ReviewResponse>? LikeReview { get; init; } = [];
    public IList<CommentShortResponse>? Comments { get; init; } = [];
    public IList<LikeCommentResponse>? LikeComment { get; init; } = [];
    public IList<SubscriptionShortResponse>? Subscriptions { get; init; } = [];
    public IList<BookmarkShortResponse>? Bookmarks { get; init; } = [];
}