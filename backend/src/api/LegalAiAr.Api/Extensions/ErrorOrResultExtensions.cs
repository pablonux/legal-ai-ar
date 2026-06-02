using ErrorOr;

namespace LegalAiAr.Api.Extensions;

/// <summary>
/// Maps <see cref="ErrorOr{T}"/> results to Minimal API <see cref="IResult"/> responses (RFC 7807).
/// </summary>
public static class ErrorOrResultExtensions
{
    public static IResult ToHttpResult<T>(this ErrorOr<T> result)
    {
        if (!result.IsError)
            return Results.Ok(result.Value);

        return ToProblem(result.Errors);
    }

    public static IResult ToHttpResult(this ErrorOr<Success> result)
    {
        if (!result.IsError)
            return Results.NoContent();

        return ToProblem(result.Errors);
    }

    public static IResult ToProblem(this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem(
                title: "Error",
                detail: "An error occurred.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (errors.Count == 1)
        {
            var error = errors[0];
            return Results.Problem(
                title: error.Code,
                detail: error.Description,
                statusCode: MapStatusCode(error.Type),
                extensions: BuildExtensions(errors));
        }

        var validationErrors = errors
            .GroupBy(e => string.IsNullOrWhiteSpace(e.Code) ? "general" : e.Code)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return Results.ValidationProblem(
            validationErrors,
            title: "Bad Request",
            detail: "One or more errors occurred.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    private static Dictionary<string, object?> BuildExtensions(List<Error> errors) =>
        new()
        {
            ["errors"] = errors.Select(e => new { e.Code, e.Description, Type = e.Type.ToString() }).ToArray()
        };

    private static int MapStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
}
