using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book-interaction")]
public class BookInteractionController(IBookInteractionService bookInteractionService) : BaseController
{
    [HttpPost("{bookId}")]
    public async Task<ActionResult<bool>> LikeBookAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await bookInteractionService.LikeBookAsync(bookId, userId.Value);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpDelete("{bookId}")]
    public async Task<ActionResult<bool>> UnlikeBookAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await bookInteractionService.UnlikeBookAsync(bookId, userId.Value);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }
}