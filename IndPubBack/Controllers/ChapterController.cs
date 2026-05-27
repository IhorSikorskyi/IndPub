using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Authorize]
[ApiController]
[Route("api/book/{bookId}/chapter")]
public class ChapterController(IChapterService chapterService) : BaseController
{
    [HttpPost("create")]
    public async Task<ActionResult<ChapterResponse>> CreateChapter(
        [FromBody] ChapterCreateRequest createRequest, [FromRoute(Name = "bookId")] Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }

            var chapter = await chapterService.CreateChapterAsync(bookId, userId.Value, createRequest);
            return Ok(chapter);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpPut("update/{chapterId}")]
    public async Task<ActionResult<ChapterResponse>> UpdateChapter(
        [FromBody] ChapterUpdateRequest updateRequest,
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }
            var chapter = await chapterService.UpdateChapterAsync(bookId, chapterId, userId.Value, updateRequest);
            return Ok(chapter);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [HttpDelete("delete/{chapterId}")]
    public async Task<ActionResult> DeleteChapter(
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(new { message = InvalidMessage });
            }
            var result = await chapterService.DeleteChapterAsync(bookId, chapterId, userId.Value);
            if (!result)
            {
                return NotFound(new { message = "Chapter not found or you are not the author" });
            }
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }

    [AllowAnonymous]
    [HttpGet("{chapterId:guid}")]
    public async Task<ActionResult<ChapterResponse>> GetChapter(
        [FromRoute(Name = "bookId")] Guid bookId,
        [FromRoute(Name = "chapterId")] Guid chapterId)
    {
        try
        {
            var chapter = await chapterService.GetChapterAsync(bookId, chapterId);
            return Ok(chapter);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = MessageStatus500 });
        }
    }
}
