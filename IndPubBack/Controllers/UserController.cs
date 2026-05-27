using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController(IUserService userService) : BaseController
    {
        private const string GenericErrorMessage = "An error occurred while processing your request.";

        [AllowAnonymous]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserInfoResponse>> GetProfileAsync(
            [FromRoute] Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var profileId = currentUserId ?? userId;

                var response = await userService.GetUserInfoAsync(profileId);

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
                return StatusCode(500, new { message = GenericErrorMessage });
            }
        }

        [HttpPut("update")]
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
                return StatusCode(500, new { message = GenericErrorMessage });
            }
        }

        [HttpDelete("delete/{userId}")]
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
                return StatusCode(500, new { message = GenericErrorMessage });
            }
        }
    }
}
