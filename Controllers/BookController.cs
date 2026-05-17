using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/book")]
    public class BookController(IBookService bookService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<BookResponse>> CreateBookAsync(
            [FromBody] BookCreateRequest request)
        {
            try
            {
                var result = await bookService.CreateBookAsync(request);
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

        [HttpPut("update/{bookId}")]
        public async Task<ActionResult<BookResponse>> UpdateBookAsync(
            [FromBody] BookUpdateRequest request,
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

                var result = await bookService.UpdateBookAsync(request, bookId, userId);
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

        [HttpDelete("delete/{bookId}")]
        public async Task<ActionResult<bool>> DeleteBookAsync(
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

                var result = await bookService.DeleteBookAsync(bookId, userId);
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

        [AllowAnonymous]
        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookResponse>> GetBookByIdAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            try
            {
                var result = await bookService.GetBookByIdAsync(bookId);
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
}