using System.Text.Json;

namespace CinemaBooking.WebApi;

public class GlobalExceptionHandlerMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request.  Check your payload and try again.",
                Instance = $"{context.Request.Method} {context.Request.Path}",
                Detail = ex.ToDetail()
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An internal server error has occurred. Please try again later.",
                Instance = $"{context.Request.Method} {context.Request.Path}",
                Detail = ex.Message
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}

public static class ExceptionHelper
{
    public static string? ToDetail(this BadHttpRequestException ex)
    {
        if (ex.InnerException is not null && ex.InnerException.GetType() == typeof(JsonException))
        {
            return ex.InnerException.Message;
        }

        return "Bad request.  Check your payload and try again.";
    }
}
