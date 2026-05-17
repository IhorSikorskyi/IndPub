using Azure.Core;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book-interaction")]
public class BookInteractionController(IBookInteractionService bookInteractionService) : ControllerBase
{
    [HttpPost("bookId")]
    public async Task<ActionResult<bool>> LikeBookAsync(
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

            var result = await bookInteractionService.LikeBookAsync(bookId, userId);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while processing your request." });
        }
    }

    [HttpDelete("bookId")]
    public async Task<ActionResult<bool>> UnlikeBookAsync(
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

            var result = await bookInteractionService.UnlikeBookAsync(bookId, userId);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while processing your request." });
        }
    }
}