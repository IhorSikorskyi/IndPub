using IndPubBack.Enums;
using System.Text.Json.Serialization;

namespace IndPubBack.DTOs.Requests.Library;

public record LibraryListRequest
{
    public LibraryBookStatus Status { get; init; } = LibraryBookStatus.Reading;

    public DateTime? Cursor { get; init; }

    [JsonRequired]
    public int PageSize { get; init; }
}