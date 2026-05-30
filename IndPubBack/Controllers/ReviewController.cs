using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book/{bookId}/review")]
public class ReviewController(IReviewService reviewService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> CreateReviewAsync(
        [FromBody] ReviewRequest createRequest, [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var review = await reviewService.CreateReviewAsync(bookId, userId.Value, createRequest);
            return Ok(review);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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

    [HttpPut("{reviewId}")]
    public async Task<ActionResult<ReviewResponse>> UpdateReviewAsync(
        [FromBody] ReviewRequest updateRequest, [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "reviewId")] Guid reviewId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var review = await reviewService.UpdateReviewAsync(reviewId, bookId, userId.Value, updateRequest);
            return Ok(review);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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

    [HttpDelete("{reviewId}")]
    public async Task<ActionResult> DeleteReviewAsync([FromRoute(Name = "reviewId")] Guid reviewId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            await reviewService.DeleteReviewAsync(userId.Value, reviewId);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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

    [HttpPost("{reviewId}/like")]
    public async Task<ActionResult> LikeReviewAsync([FromRoute(Name = "reviewId")] Guid reviewId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            bool liked = await reviewService.LikeReviewInteractionAsync(reviewId, userId.Value);
            return Ok(new { liked });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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
    [HttpGet("{reviewId}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewByIdAsync(
        [FromRoute(Name = "reviewId")] Guid reviewId)
    {
        try
        {
            var review = await reviewService.GetReviewByIdAsync(reviewId);
            return Ok(review);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAllReviewsAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var reviews = await reviewService.GetAllReviewsForBookAsync(bookId);
            return Ok(reviews);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpGet("my")]
    public async Task<ActionResult<ReviewResponse>> GetUserReviewAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var review = await reviewService.GetUserReviewAsync(bookId, userId.Value);
            return Ok(review);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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
}