using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
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
    [ProducesResponseType(typeof(UserInfoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<UserInfoResponse>> GetProfileAsync(
        [FromRoute(Name = "userId")] Guid userId)
    {
        var response = await userService.GetUserInfoAsync(userId);

        return Ok(response);
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserInfoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<UserInfoResponse>> GetUserProfileAsync()
    {
        var userId = GetCurrentUserId();

        var response = await userService.GetUserInfoAsync(userId);

        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserInfoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<UserInfoResponse>> UpdateProfileAsync(
        UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await userService.UpdateUserInfoAsync(userId, request);

        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<bool>> DeleteProfileAsync(
        [FromRoute(Name = "userId")] Guid? targetUserId)
    {
        var userId = GetCurrentUserId();

        var result = await userService.DeleteAccountAsync(userId, targetUserId);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{userId:guid}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetUserReviewsAsync(
        [FromRoute(Name = "userId")] Guid userId)
    {
        var reviews = await userService.GetAllReviewsByUserAsync(userId);

        return Ok(reviews);
    }
}