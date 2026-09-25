using IndPubBack.DTOs.Requests.Subscription;
using IndPubBack.DTOs.Responses.Error;
using IndPubBack.DTOs.Responses.User;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[Route("api/subscriptions")]
[ApiController]
public class SubscriptionController(ISubscriptionService subscriptionService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(UserActivitiesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserActivitiesResponse>> GetSubscriptionListAsync(
        SubscriptionListRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.GetSubscriptionListAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("{authorId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> SubscribeAsync(
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.SubscribeAsync(authorId, userId);
        return Ok(result);
    }

    [HttpDelete("{authorId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> UnsubscribeAsync(
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.UnsubscribeAsync(authorId, userId);
        return Ok(result);
    }
}
