using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[ApiController]
[Route("api")]
public class AuthController(IAuthService authService) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";

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
            return StatusCode(500, new { message = MessageStatus500 });
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserResponse>> Refresh()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var refreshToken = Request.Cookies[$"{RefreshTokenCookieName}"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            var result = await authService.UpdateAccessTokenAsync(userId.Value, refreshToken);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<bool>> LogoutAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var refreshToken = Request.Cookies[$"{RefreshTokenCookieName}"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            var result = await authService.LogoutAsync(userId.Value, refreshToken);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

}
