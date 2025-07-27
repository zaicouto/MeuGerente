using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Modules.Orders.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        ValidationContext<TRequest> context = new(request);

        List<ValidationFailure> failures =
        [
            .. validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f != null),
        ];

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next(CancellationToken.None);
    }
}
