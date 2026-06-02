using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Filters;

/// <summary>
/// Runs FluentValidation for endpoint-bound arguments before the handler executes.
/// Complements <see cref="LegalAiAr.Application.Common.Behaviors.ValidationBehavior{TRequest,TResponse}"/>
/// for API-layer request models.
/// </summary>
public sealed class ValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null)
                continue;

            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (result.IsValid)
                continue;

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Results.ValidationProblem(
                errors,
                title: "Bad Request",
                detail: "One or more validation errors occurred.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return await next(context);
    }
}
