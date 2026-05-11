using System.Security.Claims;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[ApiController]
[Route("api")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private const string GenericErrorMessage = "An error occurred while processing your request.";
    private const string RefreshTokenCookieName = "refreshToken";
    private const string BearerTokenCookieName = "Bearer";

    #region Authorization

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> RegisterAsync(
        [FromBody] RegisterRequest request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);

            Response.Cookies.Append($"{RefreshTokenCookieName}", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
                Expires = result.RefreshTokenExpiry
            });

            return Ok(new { accessToken = result.AccessToken });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = GenericErrorMessage });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> LoginAsync(
        [FromBody] LoginRequest request)
    {
        try
        {
            var result = await authService.LoginAsync(request);

            Response.Cookies.Append($"{RefreshTokenCookieName}", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
                Expires = result.RefreshTokenExpiry
            });

            return Ok(new { accessToken = result.AccessToken });
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = GenericErrorMessage });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserResponse>> Refresh(
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var refreshToken = Request.Cookies[$"{RefreshTokenCookieName}"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            var result = await authService.UpdateAccessTokenAsync(accessToken, refreshToken);
            return Ok(new { accessToken = result.AccessToken });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = GenericErrorMessage });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<bool>> LogoutAsync(
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            var result = await authService.LogoutAsync(accessToken, refreshToken);
            return Ok(new { success = result });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = GenericErrorMessage });
        }
    }

    #endregion
}
