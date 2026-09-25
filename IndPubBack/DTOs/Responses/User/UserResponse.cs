namespace IndPubBack.DTOs.Responses.User;

public record UserResponse
{
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; init; }
    public AccessTokenResponse AccessToken { get; init; } = new(string.Empty);
    public override string ToString() =>
        $"{nameof(UserResponse)} {{ AccessToken = [REDACTED], RefreshToken = [REDACTED], RefreshTokenExpiry = {RefreshTokenExpiry} }}";
}