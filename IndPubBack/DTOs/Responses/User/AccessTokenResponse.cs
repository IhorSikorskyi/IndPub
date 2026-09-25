namespace IndPubBack.DTOs.Responses.User;

public record AccessTokenResponse(string AccessToken)
{
    public override string ToString() => $"{nameof(AccessTokenResponse)} {{ AccessToken = [REDACTED] }}";
}