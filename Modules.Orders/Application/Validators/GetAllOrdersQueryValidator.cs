using FluentValidation;
using Modules.Orders.Application.Queries;

namespace Modules.Orders.Application.Validators;

public class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
{
    public GetAllOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero.")
            .LessThanOrEqualTo(1000)
            .WithMessage("O número da página não pode ser maior que 1000.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("O tamanho da página deve ser maior que zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("O tamanho da página não pode ser maior que 100.");
    }
}
