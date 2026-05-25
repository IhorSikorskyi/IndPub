using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers
{
    [Authorize]
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionController(ISubscriptionService subscriptionService) : BaseController
    {
        private const string GenericErrorMessage = "An error occurred while processing your request.";

        [HttpGet]
        public async Task<ActionResult<UserActivitiesResponse>> GetSubscriptionListAsync(
            [FromQuery] SubscriptionListRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await subscriptionService.GetSubscriptionListAsync(userId.Value, request);
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

        [HttpPost("{authorId}")]
        public async Task<ActionResult<bool>> SubscribeAsync(
            [FromRoute(Name = "authorId")] Guid authorId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await subscriptionService.SubscribeAsync(authorId, userId.Value);
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

        [HttpDelete("{authorId}")]
        public async Task<ActionResult<bool>> UnsubscribeAsync(
            [FromRoute(Name = "authorId")] Guid authorId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await subscriptionService.UnsubscribeAsync(authorId, userId.Value);
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

        [HttpGet("isSubscribed/{authorId}")]
        public async Task<ActionResult<bool>> IsSubscribedAsync(
            [FromRoute(Name = "authorId")] Guid authorId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await subscriptionService.IsSubscribedAsync(userId.Value, authorId);
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


