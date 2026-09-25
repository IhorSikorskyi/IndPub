using IndPubBack.DTOs.Requests.Book;
using IndPubBack.DTOs.Responses.Book;
using IndPubBack.DTOs.Responses.Error;
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
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookResponse>> CreateBookAsync(
            BookCreateRequest request)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.CreateBookAsync(request, userId);
            return Ok(result);
        }

        [HttpPut("{bookId:guid}")]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookResponse>> UpdateBookAsync(
            BookUpdateRequest request,
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.UpdateBookAsync(request, bookId, userId);

            return Ok(result);
        }

        [HttpDelete("{bookId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteBookAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.DeleteBookAsync(bookId, userId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{bookId:guid}")]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookResponse>> GetBookByIdAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var result = await bookService.GetBookByIdAsync(bookId);
            return Ok(result);
        }
    }
}