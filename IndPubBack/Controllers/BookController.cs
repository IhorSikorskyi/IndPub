using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
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
        [ProducesResponseType(typeof(BookResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<BookResponse>> CreateBookAsync(
            BookCreateRequest request)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.CreateBookAsync(request, userId);
            return Ok(result);
        }

        [HttpPut("{bookId:guid}")]
        [ProducesResponseType(typeof(BookResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<BookResponse>> UpdateBookAsync(
            BookUpdateRequest request,
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.UpdateBookAsync(request, bookId, userId);

            return Ok(result);
        }

        [HttpDelete("{bookId:guid}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<bool>> DeleteBookAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var userId = GetCurrentUserId();

            var result = await bookService.DeleteBookAsync(bookId, userId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{bookId:guid}")]
        [ProducesResponseType(typeof(BookResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<BookResponse>> GetBookByIdAsync(
            [FromRoute(Name = "bookId")] Guid bookId)
        {
            var result = await bookService.GetBookByIdAsync(bookId);
            return Ok(result);
        }
    }
}