namespace IndPubBack.DTO.Responses;

public class ChapterResponse // Update and Create use the same response
{
    // UserId from ClaimsPrincipal, so we don't need it here
}

public class ChapterShortResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int ChapterNumber { get; set; }
}

public class LikeChapterResponse // This is used for both like and unlike, as the client can determine the action based on the IsLiked property
{
    // UserId from ClaimsPrincipal, so we don't need it here
    public Guid ChapterId { get; set; }
    public bool IsLiked { get; set; }
}