using IndPubBack.DTOs.Requests;
using IndPubBack.DTOs.Responses;
using IndPubBack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

[Route("api/search")]
[ApiController]
public class SearchController(ISearchService searchService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<BookShortResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IList<BookShortResponse>>> GetBooksByFiltersAsync(
        BookSearchRequest request)
    {
        var result = await searchService.GetBooksByFiltersAsync(request);
        return Ok(result);
    }
}