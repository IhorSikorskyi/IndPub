using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[ApiController]
[Route("api")]
public class UserController(IUserService userService) : ControllerBase
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
            var result = await userService.RegisterAsync(request);

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
            var result = await userService.LoginAsync(request);

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

            var result = await userService.UpdateAccessTokenAsync(accessToken, refreshToken);
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

            var result = await userService.LogoutAsync(accessToken, refreshToken);
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

    #region Profile

    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult<UserInfoResponse>> GetOwnProfileAsync(
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.GetUserInfoAsync(accessToken, Guid.Empty);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [AllowAnonymous]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserInfoResponse>> GetUserProfileAsync(
        [FromHeader(Name = "Authorization")] string? authorization,
        [FromRoute(Name = "userId")] Guid userId)
    {
        try
        {
            var accessToken = authorization?.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.GetUserInfoAsync(accessToken, userId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
    [HttpPut("user/update")]
    public async Task<ActionResult<UserInfoResponse>> UpdateProfileAsync(
        [FromBody] UpdateProfileRequest request,
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            if (string.IsNullOrEmpty(authorization))
            {
                return Unauthorized();
            }

            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.UpdateUserInfoAsync(accessToken, request);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
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

    [Authorize]
    [HttpDelete("user/delete")]
    [HttpDelete("user/delete/{userId}")]
    public async Task<ActionResult<bool>> DeleteProfileAsync(
        [FromHeader(Name = "Authorization")] string authorization,
        [FromRoute(Name = "userId")] Guid? userId)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.DeleteAccountAsync(accessToken, userId);
            return Ok(result);
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

    #region Subscription

    [Authorize]
    [HttpGet("user/subscribes")]
    public async Task<ActionResult<IList<BookShortResponse>>> GetSubscriptionListAsync(
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.GetSubscriptionListAsync(accessToken);
            return Ok(result);
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
    [HttpPost("user/{authorId}")]
    public async Task<ActionResult<bool>> SubscribeAsync(
        [FromHeader(Name = "Authorization")] string authorization,
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.SubscribeAsync(authorId, accessToken);
            return Ok(result);
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
    [HttpDelete("user/{authorId}")]
    [HttpDelete("user/subscribes/{authorId}")]
    public async Task<ActionResult<bool>> UnsubscribeAsync(
        [FromHeader(Name = "Authorization")] string authorization,
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.UnsubscribeAsync(authorId, accessToken);
            return Ok(result);
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

    #region Library

    [Authorize]
    [HttpGet("user/library")]
    public async Task<ActionResult<IList<BookResponse>>> GetLibraryAsync(
        [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.GetLibraryAsync(accessToken);
            return Ok(result);
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
    [HttpPost("book/{bookId}")]
    public async Task<ActionResult<bool>> AddToLibraryAsync(
        [FromHeader(Name = "Authorization")] string authorization, 
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.AddToLibraryAsync(bookId, accessToken);

            return Ok(result);
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
    [HttpDelete("book/{bookId}")]
    [HttpDelete("user/library/{bookId}")]
    public async Task<ActionResult<bool>> RemoveFromLibraryAsync(
        [FromHeader(Name = "Authorization")] string authorization,
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var accessToken = authorization.Replace($"{BearerTokenCookieName} ", "");
            var result = await userService.RemoveFromLibraryAsync(bookId, accessToken);

            return Ok(result);
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
