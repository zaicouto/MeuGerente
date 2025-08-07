using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Modules.Orders.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    Serilog.ILogger logger
) : IPipelineBehavior<TRequest, TResponse>
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

        logger.Information("Requisição {RequestType} validada.", typeof(TRequest).Name);
        return await next(CancellationToken.None);
    }
}
