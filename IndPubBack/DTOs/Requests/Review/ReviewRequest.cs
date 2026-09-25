namespace IndPubBack.DTOs.Requests.Review;

public record ReviewRequest(double? Rating = null, string? ReviewText = null);