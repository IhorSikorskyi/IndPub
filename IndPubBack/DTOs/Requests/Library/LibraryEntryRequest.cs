using IndPubBack.Enums;
using System.Text.Json.Serialization;

namespace IndPubBack.DTOs.Requests.Library;

public record LibraryEntryRequest
{
    [JsonRequired]
    public Guid BookId { get; init; }

    [JsonRequired]
    public LibraryBookStatus Status { get; init; }
}