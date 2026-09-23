using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[Route("api/library")]
[ApiController]
public class LibraryController(ILibraryService libraryService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<UserActivitiesResponse>), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<IList<UserActivitiesResponse>>> GetLibraryAsync(LibraryListRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await libraryService.GetLibraryAsync(userId, request);

        return Ok(result);
    }

    [HttpPost("{bookId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> AddToLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var result = await libraryService.AddToLibraryAsync(bookId, userId);

        return Ok(result);
    }

    [HttpDelete("{bookId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> RemoveFromLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var result = await libraryService.RemoveFromLibraryAsync(bookId, userId);

        return Ok(result);
    }

    [HttpGet("isInLibrary/{bookId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> IsBookInLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var result = await libraryService.IsBookInLibraryAsync(userId, bookId);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<bool>> UpdateLibraryEntryStatusAsync(LibraryEntryRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await libraryService.UpdateLibraryEntryStatusAsync(userId, request);

        return Ok(result);
    }
}

