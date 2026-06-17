using System.Text.Json;
using FluentValidation;

namespace PharmacyBilling.API.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                error = "Validation failed",
                errors
            }));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Business rule violation: {Message}", ex.Message);
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                error = "An internal server error occurred."
            }));
        }
    }
}
