using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IndPubBack.Exceptions;

namespace IndPubBack.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(IUserService _userService) : ControllerBase
    {
        private const string GenericErrorMessage = "An error occurred while processing your request.";

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> RegisterAsync([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _userService.RegisterAsync(request);

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
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
        public async Task<ActionResult<UserResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _userService.LoginAsync(request);

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
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
        public async Task<ActionResult<UserResponse>> Refresh([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var accessToken = authorization?.Replace("Bearer ", "") ?? string.Empty;
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token cookie is missing." });
                }
                var result = await _userService.UpdateAccessTokenAsync(accessToken, refreshToken);
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
        public async Task<ActionResult<bool>> LogoutAsync([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var accessToken = authorization?.Replace("Bearer ", "") ?? string.Empty;
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token cookie is missing." });
                }
                var result = await _userService.LogoutAsync(accessToken, refreshToken);
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserInfoResponse>> GetProfileAsync([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            { 
                var accessToken = authorization?.Replace("Bearer ", "") ?? string.Empty;
                var result = await _userService.GetUserInfoAsync(accessToken);
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
        [HttpPut]
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

                var accessToken = authorization.Replace("Bearer ", "");
                var result = await _userService.UpdateUserInfoAsync(accessToken, request);
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

    }
}