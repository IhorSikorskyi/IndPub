using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> RegisterAsync(
        [FromBody] RegisterRequest request)
    {
        try
        {
            var result  = await authService.RegisterAsync(request);

            Response.Cookies.Append(RefreshTokenCookieName, result.refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
                Expires = result.refreshTokenExpiry
            });

            return Ok(new { accessToken = result.response });
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

            Response.Cookies.Append(RefreshTokenCookieName, result.refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
                Expires = result.refreshTokenExpiry
            });

            return Ok(new { accessToken = result.response });
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
    public async Task<ActionResult<UserResponse>> Refresh(
        [FromHeader(Name = "Authorization")] string? authorization)
    {
        try
        {
            if (string.IsNullOrEmpty(authorization))
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var accessToken = authorization.Replace("Bearer ", "");

            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            var result = await authService.UpdateAccessTokenAsync(accessToken, refreshToken);

            Response.Cookies.Append(RefreshTokenCookieName, result.refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict, // Change to None, if you need cross-site cookies
                Expires = result.refreshTokenExpiry
            });

            return Ok(new { accessToken = result.response });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (SecurityException ex)
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
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token cookie is missing." });
            }

            await authService.LogoutAsync(refreshToken);

            Response.Cookies.Delete(RefreshTokenCookieName);

            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (SecurityException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }
}
