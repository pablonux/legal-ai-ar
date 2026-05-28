using System.Net;
using System.Text.Json;
using FluentValidation;
using LegalAiAr.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions and returns RFC 7807 ProblemDetails responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private const string ProblemDetailsContentType = "application/problem+json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug("Request cancelled by client: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                CreateValidationProblemDetails(context, validationEx)),

            NotFoundException => (
                HttpStatusCode.NotFound,
                CreateProblemDetails(
                    context,
                    HttpStatusCode.NotFound,
                    "Not Found",
                    exception.Message)),

            DomainException => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails(
                    context,
                    HttpStatusCode.BadRequest,
                    "Bad Request",
                    exception.Message)),

            _ => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Internal Server Error",
                    _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred."))
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogDebug(exception, "Handled exception: {Message}", exception.Message);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = ProblemDetailsContentType;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, JsonOptions));
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Bad Request",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };
    }
}
