using System.Text.Json;
using EventApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventApi.Middleware;

/// <summary>
/// Middleware глобальной обработки исключений.
/// Перехватывает все необработанные исключения и возвращает единообразный
/// JSON-ответ в формате Problem Details (RFC 7807).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = MapException(exception);

        // Ожидаемые ошибки клиента логируются как предупреждения,
        // непредвиденные ошибки сервера — как ошибки.
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Необработанное исключение при обработке запроса {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Ошибка обработки запроса {Method} {Path}: {Message}",
                context.Request.Method,
                context.Request.Path,
                exception.Message);
        }

        // Если ответ уже начал отправляться, изменить его невозможно — остаётся только записать ошибку в лог.
        if (context.Response.HasStarted)
        {
            return;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Headers.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, SerializerOptions));
    }

    /// <summary>
    /// Сопоставляет тип исключения с HTTP-статусом и сообщением для клиента.
    /// Для непредвиденных ошибок детали не раскрываются, чтобы не утекала внутренняя информация.
    /// </summary>
    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) => exception switch
    {
        ValidationException => (
            StatusCodes.Status400BadRequest,
            "Ошибка валидации",
            exception.Message),
        NotFoundException => (
            StatusCodes.Status404NotFound,
            "Ресурс не найден",
            exception.Message),
        _ => (
            StatusCodes.Status500InternalServerError,
            "Внутренняя ошибка сервера",
            "Произошла непредвиденная ошибка. Попробуйте повторить запрос позже.")
    };
}
