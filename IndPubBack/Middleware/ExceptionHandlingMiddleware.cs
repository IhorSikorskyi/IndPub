using System.Net;
using System.Text.Json;
using IndPubBack.Exceptions;

namespace IndPubBack.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        logger.LogError(
            exception,
            "Unhandled exception of type {ExceptionType} while processing request {Method} {Path}",
            exception.GetType().Name,
            context.Request.Method,
            context.Request.Path);

        var problemDetails = new
        {
            status = (int)statusCode,
            title,
            detail = environment.IsDevelopment() ? exception.Message : null,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation error"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict error"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, "Invalid credentials"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden access"),
            SecurityException => (HttpStatusCode.Forbidden, "Security error"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };
}