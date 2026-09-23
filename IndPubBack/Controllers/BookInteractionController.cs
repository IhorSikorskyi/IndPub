using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book-interaction")]
public class BookInteractionController(IBookInteractionService bookInteractionService) : BaseController
{
    [HttpPost("{bookId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> LikeBookAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var result = await bookInteractionService.LikeBookAsync(bookId, userId);
        return Ok(result);
    }

    [HttpDelete("{bookId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> UnLikeBookAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var result = await bookInteractionService.UnLikeBookAsync(bookId, userId);
        return Ok(result);
    }
}