using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 409)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<AccessTokenResponse>> RegisterAsync(
        RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        SetRefreshTokenCookie(result.refreshToken, result.refreshTokenExpiry);

        var accessToken = result.response.AccessToken;

        return Ok(new AccessTokenResponse(accessToken));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<AccessTokenResponse>> LoginAsync(
        LoginRequest request)
    {

        var result = await authService.LoginAsync(request);

        SetRefreshTokenCookie(result.refreshToken, result.refreshTokenExpiry);

        var accessToken = result.response.AccessToken;

        return Ok(new AccessTokenResponse(accessToken));

    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<AccessTokenResponse>> Refresh(
        [FromHeader(Name = "Authorization")] string? authorization)
    {
        if (string.IsNullOrEmpty(authorization) ||
            !authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException(MissingOrInvalidTokenMessage);
        }

        var accessToken = authorization[BearerPrefix.Length..].Trim();

        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ValidationException("Refresh token cookie is missing.");
        }

        var result = await authService.UpdateAccessTokenAsync(accessToken, refreshToken);

        SetRefreshTokenCookie(result.refreshToken, result.refreshTokenExpiry);

        return Ok(new AccessTokenResponse(result.response.AccessToken));
    }

    [HttpPost("logout")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> LogoutAsync()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ValidationException("Refresh token cookie is missing.");
        }

        await authService.LogoutAsync(refreshToken);
        Response.Cookies.Delete(RefreshTokenCookieName);

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTimeOffset expiry)
    {
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
            Expires = expiry
        });
    }
}
