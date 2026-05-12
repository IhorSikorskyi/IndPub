using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers;

[Authorize]
[Route("api/library")]
[ApiController]
public class LibraryController(ILibraryService libraryService) : ControllerBase
{
    private const string GenericErrorMessage = "An error occurred while processing your request.";
    private const string TokenInvalidMessage = "Invalid user id in token.";

    [HttpGet]
    public async Task<ActionResult<IList<UserActivitiesResponse>>> GetLibraryAsync(LibraryListRequest request)
    {
        try
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new { message = $"{TokenInvalidMessage}" });
            }

            var result = await libraryService.GetLibraryAsync(userId, request);

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

    [HttpPost("{bookId}")]
    public async Task<ActionResult<bool>> AddToLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new { message = $"{TokenInvalidMessage}" });
            }

            var result = await libraryService.AddToLibraryAsync(bookId, userId);

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

    [HttpDelete("{bookId}")]
    public async Task<ActionResult<bool>> RemoveFromLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new { message = $"{TokenInvalidMessage}" });
            }

            var result = await libraryService.RemoveFromLibraryAsync(bookId, userId);

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

    [HttpGet("check/{bookId}")]
    public async Task<ActionResult<bool>> IsBookInLibraryAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new { message = "Invalid user id in token." });
            }

            var result = await libraryService.IsBookInLibraryAsync(userId, bookId);
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

    [HttpPost]
    public async Task<ActionResult<bool>> UpdateLibraryEntryStatusAsync([FromBody] LibraryEntryRequest request)
    {
        try
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new { message = $"{TokenInvalidMessage}" });
            }

            var result = await libraryService.UpdateLibraryEntryStatusAsync(userId, request);

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

