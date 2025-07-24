using FluentValidation;
using Modules.Orders.Application.Commands;

namespace Modules.Orders.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(c => c.Items).NotEmpty().WithMessage("Ao menos um campo é obrigatório.");

        RuleForEach(c => c.Items)
            .ChildRules(items =>
            {
                items
                    .RuleFor(i => i.ProductName)
                    .NotEmpty()
                    .WithMessage("O nome do produto é obrigatório.");

                items
                    .RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("A quantidade deve ser maior do que zero.");

                items
                    .RuleFor(i => i.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("O preço do produto deve ser maior do que zero.");
            });
    }
}
