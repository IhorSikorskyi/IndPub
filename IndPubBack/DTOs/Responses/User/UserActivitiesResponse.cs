using IndPubBack.DTOs.Responses.Book;
using IndPubBack.DTOs.Responses.Comment;
using IndPubBack.DTOs.Responses.Library;
using IndPubBack.DTOs.Responses.Review;
using IndPubBack.DTOs.Responses.Subscription;

namespace IndPubBack.DTOs.Responses.User;

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