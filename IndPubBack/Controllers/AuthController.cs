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
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccessTokenResponse>> RegisterAsync(
        RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiry);

        var accessToken = result.AccessToken;

        return Ok(accessToken);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccessTokenResponse>> LoginAsync(
        LoginRequest request)
    {

        var result = await authService.LoginAsync(request);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiry);

        var accessToken = result.AccessToken;

        return Ok(accessToken);

    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiry);

        return Ok(result.AccessToken);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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
