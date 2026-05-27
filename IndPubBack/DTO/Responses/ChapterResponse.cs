namespace IndPubBack.DTO.Responses;

public class ChapterResponse
{
    public Guid BookId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int ChapterNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChapterShortResponse
{
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = null!;
    public int ChapterNumber { get; set; }
}