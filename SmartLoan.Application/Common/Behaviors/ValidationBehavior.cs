using FluentValidation;
using MediatR;

namespace SmartLoan.Application.Common.Behaviors;

// This is a "Generic" behavior. It works for ANY Command (TRequest)
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Run all validators for this specific command
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // If there are any errors, throw a ValidationException
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        // If everything is fine, move to the next step (The Handler/Chef)
        return await next();
    }
}