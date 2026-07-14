using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using Microsoft.AspNetCore.Diagnostics;

namespace LearningPlatform.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, response) = MapException(exception);

        if (exception is AppException)
        {
            logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }
        else
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private static (int StatusCode, ApiResponse Response) MapException(Exception exception) =>
        exception switch
        {
            ValidationException validationException => (
                (int)validationException.StatusCode,
                ApiResponse.Fail(FlattenValidationErrors(validationException), validationException.Message)),

            AppException appException => (
                (int)appException.StatusCode,
                ApiResponse.Fail(appException.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Fail("An unexpected error occurred. Please try again later."))
        };

    private static List<string> FlattenValidationErrors(ValidationException exception) =>
        exception.Errors
            .SelectMany(e => e.Value.Select(message => $"{e.Key}: {message}"))
            .ToList();
}
