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
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime JoiningDate { get; set; }

}

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}

public class UserDashboardResponse
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Role { get; set; } = string.Empty;
    public int SubscribersCount { get; set; }

    public IList<LibraryEntryResponse> Library { get; set; } = [];
    public IList<BookShortResponse> LikedBooks { get; set; } = [];
    public IList<ReviewShortResponse> Reviews { get; set; } = [];
    public IList<CommentShortResponse> Comments { get; set; } = [];
    public IList<SubscriptionShortResponse> Subscriptions { get; set; } = [];
    public IList<BookmarkShortResponse> Bookmarks { get; set; } = [];
}

public class LibraryEntryResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public LibraryBookStatus Status { get; set; }
}

public class AddToLibraryResponse
{
    public Guid BookId { get; set; }
    public bool IsAdded { get; set; }
}

// For Delete User or Remove from Library we use DeleteResponse
// For Delete User or Remove from Library we can use 204 No Content, so we don't need specific responses for those operations

public class NotificationResponse
{
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubscriptionResponse // Update and Create use the same response
{
    // UserId from ClaimsPrincipal, so we don't need it here
}

public class SubscriptionShortResponse
{
    public Guid AuthorId { get; set; }
    public string AuthorLogin { get; set; } = string.Empty;
    public string? AuthorProfilePictureUrl { get; set; }
}