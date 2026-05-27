namespace IndPubBack.DTO.Requests;

public class ChapterCreateRequest
{
    public string? Title { get; set; }
    public required string Content { get; set; }
    public int? ChapterNumber { get; set; }
}

public class ChapterUpdateRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
}