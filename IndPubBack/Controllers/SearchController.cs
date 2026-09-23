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
    [ProducesResponseType(typeof(IList<BookShortResponse>), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<IList<BookShortResponse>>> GetBooksByFiltersAsync(
        BookSearchRequest request)
    {
        var result = await searchService.GetBooksByFiltersAsync(request);
        return Ok(result);
    }
}