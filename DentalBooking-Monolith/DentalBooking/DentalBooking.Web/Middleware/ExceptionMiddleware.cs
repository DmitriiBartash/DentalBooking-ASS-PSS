using DentalBooking.Application.Common.Exceptions;

namespace DentalBooking.Web.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex switch
            {
                BusinessValidationException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = context.Response.StatusCode
            });
        }
    }
}
