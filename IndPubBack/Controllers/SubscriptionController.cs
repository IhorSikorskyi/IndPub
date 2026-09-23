using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
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
    [ProducesResponseType(typeof(UserActivitiesResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<UserActivitiesResponse>> GetSubscriptionListAsync(
        SubscriptionListRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.GetSubscriptionListAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("{authorId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> SubscribeAsync(
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.SubscribeAsync(authorId, userId);
        return Ok(result);
    }

    [HttpDelete("{authorId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> UnsubscribeAsync(
        [FromRoute(Name = "authorId")] Guid authorId)
    {
        var userId = GetCurrentUserId();

        var result = await subscriptionService.UnsubscribeAsync(authorId, userId);
        return Ok(result);
    }
}
