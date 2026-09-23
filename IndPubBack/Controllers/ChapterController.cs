using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book/{bookId:guid}/chapter")]
public class ChapterController(IChapterService chapterService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ChapterResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChapterResponse>> CreateChapterAsync(
        ChapterCreateRequest createRequest,
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var chapter = await chapterService.CreateChapterAsync(bookId, userId, createRequest);
        return Ok(chapter);
    }

    [HttpPut("{chapterId:guid}")]
    [ProducesResponseType(typeof(ChapterResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChapterResponse>> UpdateChapterAsync(
        ChapterUpdateRequest updateRequest,
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        var userId = GetCurrentUserId();

        var chapter = await chapterService.UpdateChapterAsync(bookId, chapterId, userId, updateRequest);
        return Ok(chapter);
    }

    [HttpDelete("{chapterId:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> DeleteChapterAsync(
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        var userId = GetCurrentUserId();

        var result = await chapterService.DeleteChapterAsync(bookId, chapterId, userId);
        
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{chapterId:guid}")]
    [ProducesResponseType(typeof(ChapterResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChapterResponse>> GetChapterAsync(
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        var chapter = await chapterService.GetChapterAsync(bookId, chapterId);
        return Ok(chapter);
    }
}
