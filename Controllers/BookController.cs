using System.Security.Claims;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Controllers{

    [ApiController]
    [Route("api/book")]
    public class BookController(IBookService _bookService) : ControllerBase
    {
        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<BookCreateResponse>> CreateBookAsync([FromBody] BookCreateRequest request)
        {
            try
            {
                var result = await _bookService.CreateBookAsync(request);
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

        [Authorize]
        [HttpPut("update/{bookId}")]
        public async Task<ActionResult<BookUpdateResponse>> UpdateBookAsync([FromBody] BookUpdateRequest request, [FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdValue, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user id in token." });
                }

                var result = await _bookService.UpdateBookAsync(request, userId);
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

        [Authorize]
        [HttpDelete("delete/{bookId}")]
        public async Task<ActionResult<BookDeleteResponse>> DeleteBookAsync([FromBody] BookDeleteRequest request, [FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdValue, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user id in token." });
                }

                var result = await _bookService.DeleteBookAsync(request, userId);
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