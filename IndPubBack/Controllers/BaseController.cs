using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers;

public abstract class BaseController : ControllerBase
{
    protected const string InvalidMessage = "Invalid user id in token.";
    protected const string MessageStatus500 = "An error occurred while processing your request.";

    protected Guid? GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var id) ? id : null;
    }
}