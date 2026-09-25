using System.Text.Json.Serialization;

namespace IndPubBack.DTOs.Responses.Error;

public record ErrorResponse
{
    [JsonPropertyName("status")]
    public required int Status { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("detail")]
    public required string Detail { get; init; }

    [JsonPropertyName("traceId")]
    public required string TraceId { get; init; }
}