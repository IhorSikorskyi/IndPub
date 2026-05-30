using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
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
    public async Task<ActionResult<IList<UserActivitiesResponse>>> GetLibraryAsync([FromQuery] LibraryListRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await libraryService.GetLibraryAsync(userId.Value, request);

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
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpPost("{bookId}")]
    public async Task<ActionResult<bool>> AddToLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await libraryService.AddToLibraryAsync(bookId, userId.Value);

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
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpDelete("{bookId}")]
    public async Task<ActionResult<bool>> RemoveFromLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await libraryService.RemoveFromLibraryAsync(bookId, userId.Value);

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
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpGet("isInLibrary/{bookId}")]
    public async Task<ActionResult<bool>> IsBookInLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await libraryService.IsBookInLibraryAsync(userId.Value, bookId);
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
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpPut]
    public async Task<ActionResult<bool>> UpdateLibraryEntryStatusAsync([FromQuery] LibraryEntryRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var result = await libraryService.UpdateLibraryEntryStatusAsync(userId.Value, request);

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

