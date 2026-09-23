using IndPubBack.Entities;
using System.Text.Json.Serialization;
using IndPubBack.Enums;

namespace IndPubBack.DTOs.Requests;

public record LibraryListRequest
{
    public LibraryBookStatus Status { get; init; } = LibraryBookStatus.Reading;

    public DateTime? Cursor { get; init; }

    [JsonRequired]
    public int PageSize { get; init; }
}

public record LibraryEntryRequest
{
    [JsonRequired]
    public Guid BookId { get; init; }

    [JsonRequired]
    public LibraryBookStatus Status { get; init; }
}