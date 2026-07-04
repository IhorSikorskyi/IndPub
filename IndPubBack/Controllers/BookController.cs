using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/book")]
    public class BookController(IBookService bookService) : BaseController
    {
        [HttpPost]
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
                return StatusCode(500, new { message = MessageStatus500 });
            }
        }

        [HttpPut("{bookId}")]
        public async Task<ActionResult<BookResponse>> UpdateBookAsync(
            [FromBody] BookUpdateRequest request,
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await bookService.UpdateBookAsync(request, bookId, userId.Value);
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
        public async Task<ActionResult<bool>> DeleteBookAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId is null)
                {
                    return Unauthorized(new { message = InvalidMessage });
                }

                var result = await bookService.DeleteBookAsync(bookId, userId.Value);
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