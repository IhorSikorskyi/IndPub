using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Exceptions;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers;

[Route("api/search")]
[ApiController]
public class SearchController(ISearchService searchService) : BaseController
{
    private const string GenericErrorMessage = "An error occurred while processing your request.";

    [HttpGet]
    public async Task<ActionResult<IList<BookShortResponse>>> GetBooksByFiltersAsync(
        [FromQuery] BookSearchRequest request)
    {
        try
        {
            var result = await searchService.GetBooksByFiltersAsync(request);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = GenericErrorMessage });
        }
    }
}