using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : BaseController
{
    [AllowAnonymous]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserInfoResponse>> GetProfileAsync(
        [FromRoute] Guid userId)
    {
        try
        {
            var response = await userService.GetUserInfoAsync(userId);

            return Ok(response);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserInfoResponse>> GetUserProfileAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var response = await userService.GetUserInfoAsync(userId.Value);

            return Ok(response);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpPut]
    public async Task<ActionResult<UserInfoResponse>> UpdateProfileAsync(
        [FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await userService.UpdateUserInfoAsync(userId.Value, request);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult<bool>> DeleteProfileAsync(
        [FromRoute(Name = "userId")] Guid? targetUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await userService.DeleteAccountAsync(userId.Value, targetUserId);
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
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [AllowAnonymous]
    [HttpGet("{userId:guid}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetUserReviewsAsync(
        [FromRoute(Name = "userId")] Guid userId)
    {
        try
        {
            var reviews = await userService.GetAllReviewsByUserAsync(userId);
            return Ok(reviews);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }
}