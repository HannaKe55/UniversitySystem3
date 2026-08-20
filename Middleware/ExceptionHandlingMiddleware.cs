using System.Net;
using System.Text.Json;
using UniversitySystem3.Common;
using UniversitySystem3.Common.Exceptions;
namespace UniversitySystem3.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (AppException ex)
        {
            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        var source = $"{ex.SourceClass}.{ex.SourceMethod} (line {ex.SourceLine})";

        _logger.LogError(ex,
            "خطای شناخته‌شده [{ErrorType}] در {Source}: {Message}",
            ex.ErrorType, source, ex.Message);

        var response = new ErrorResponseDto
        {
            Message = ex.Message,
            ErrorType = ex.ErrorType,
            StatusCode = ex.StatusCode,
            Source = source
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.StatusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "خطای پیش‌بینی‌نشده: {Message}", ex.Message);

        var response = new ErrorResponseDto
        {
            Message = "خطای داخلی سرور رخ داده است.",
            ErrorType = "InternalServerError",
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Source = ex.TargetSite?.DeclaringType?.FullName ?? "Unknown"
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
