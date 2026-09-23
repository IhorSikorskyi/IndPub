using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book/{bookId:guid}/review")]
public class ReviewController(IReviewService reviewService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewResponse>> CreateReviewAsync(
        ReviewRequest createRequest,
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var review = await reviewService.CreateReviewAsync(bookId, userId, createRequest);
        return Ok(review);
    }

    [HttpPut("{reviewId:guid}")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewResponse>> UpdateReviewAsync(
        ReviewRequest updateRequest,
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "reviewId")] Guid reviewId)
    {
        var userId = GetCurrentUserId();

        var review = await reviewService.UpdateReviewAsync(reviewId, bookId, userId, updateRequest);
        return Ok(review);
    }

    [HttpDelete("{reviewId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteReviewAsync([FromRoute(Name = "reviewId")] Guid reviewId)
    {
        var userId = GetCurrentUserId();

        await reviewService.DeleteReviewAsync(userId, reviewId);

        return NoContent();
    }

    [HttpPost("{reviewId:guid}/like")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LikeReviewAsync([FromRoute(Name = "reviewId")] Guid reviewId)
    {
        var userId = GetCurrentUserId();

        bool liked = await reviewService.LikeReviewAsync(reviewId, userId);
        return Ok(new { liked });
    }

    [HttpDelete("{reviewId:guid}/like")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnLikeReviewAsync([FromRoute(Name = "reviewId")] Guid reviewId)
    {
        var userId = GetCurrentUserId();

        bool liked = await reviewService.UnLikeReviewAsync(reviewId, userId);
        return Ok(new { liked });
    }

    [AllowAnonymous]
    [HttpGet("{reviewId:guid}")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewResponse>> GetReviewByIdAsync(
        [FromRoute(Name = "reviewId")] Guid reviewId)
    {
        var review = await reviewService.GetReviewByIdAsync(reviewId);
        return Ok(review);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAllReviewsAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var reviews = await reviewService.GetAllReviewsForBookAsync(bookId);
        return Ok(reviews);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewResponse>> GetUserReviewAsync(
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var review = await reviewService.GetUserReviewAsync(bookId, userId);
        return Ok(review);
    }
}