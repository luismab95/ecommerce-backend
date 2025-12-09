using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ecommerce.Domain.DTOs.General;

public class ErrorResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorResponseLoggingMiddleware> _logger;

    public ErrorResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<ErrorResponseLoggingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error: {Message}", ex.Message);

            await HandleErrorAsync(context, ex);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Aquí determinas el status adecuado
        var statusCode = exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var exceptionMessage = exception switch
        {
            InvalidOperationException => exception.Message,
            ArgumentException => exception.Message,
            KeyNotFoundException => exception.Message,
            _ => "Ha ocurrido un error inesperado."
        };

        context.Response.StatusCode = statusCode;

        var error = new ErrorResponse
        {
            Status = statusCode,
            Error = exception.GetType().Name,
            Message = exceptionMessage,
            TraceId = context.TraceIdentifier
        };

        var result = new GeneralResponse
        {
            Data = error,
            Message = exceptionMessage,
        };

        var json = JsonSerializer.Serialize(result);

        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Error { get; set; } = "";
    public string Message { get; set; } = "";
    public string TraceId { get; set; } = "";
}
