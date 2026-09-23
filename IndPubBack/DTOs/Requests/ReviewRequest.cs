namespace IndPubBack.DTOs.Requests;

public record ReviewRequest(double? Rating = null, string? ReviewText = null);