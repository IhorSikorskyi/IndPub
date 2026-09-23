using IndPubBack.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndPubBack.Controllers;

public abstract class BaseController : ControllerBase
{
    protected const string MissingOrInvalidTokenMessage = "Authorization header is missing or invalid.";
    protected const string InvalidUserIdInTokenMessage = "Invalid user id in token.";
    protected const string MessageStatus500 = "An error occurred while processing your request.";
    protected const string BearerPrefix = "Bearer ";


    protected Guid GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedException(InvalidUserIdInTokenMessage);
    }
}