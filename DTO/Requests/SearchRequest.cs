using IndPubBack.Models;

namespace IndPubBack.DTO.Requests;

public class BookNameSearchRequest
{
    public required string BookName { get; set; }
}

public class UserNameSearchRequest
{
    public required string UserName { get; set; }
}

public class AdvancedSearchRequest
{
    public string? Title { get; set; }

    public string? AuthorName { get; set; }

    public string? Genre { get; set; }

    public List<Guid>? TagIds { get; set; }

    public string? Language { get; set; }

    public Status? Status { get; set; }
}