using IndPubBack.Entities;

namespace IndPubBack.DTOs.Requests;

public class LibraryListRequest
{
    public LibraryBookStatus Status = LibraryBookStatus.Reading;

    public DateTime? Cursor { get; set; }

    public int PageSize { get; set; }
}

public class LibraryEntryRequest
{
    public Guid BookId { get; set; }
    public LibraryBookStatus Status { get; set; }
}