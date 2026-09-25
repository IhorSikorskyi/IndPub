using IndPubBack.DTOs.Requests.Chapter;
using IndPubBack.DTOs.Responses.Chapter;
using IndPubBack.DTOs.Responses.Error;
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
    [ProducesResponseType(typeof(ChapterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ChapterResponse>> CreateChapterAsync(
        ChapterCreateRequest createRequest,
        [FromRoute(Name = "bookId")] Guid bookId)
    {
        var userId = GetCurrentUserId();

        var chapter = await chapterService.CreateChapterAsync(bookId, userId, createRequest);
        return Ok(chapter);
    }

    [HttpPut("{chapterId:guid}")]
    [ProducesResponseType(typeof(ChapterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(ChapterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ChapterResponse>> GetChapterAsync(
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        var chapter = await chapterService.GetChapterAsync(bookId, chapterId);
        return Ok(chapter);
    }
}
