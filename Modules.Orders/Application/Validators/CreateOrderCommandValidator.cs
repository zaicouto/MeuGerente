using FluentValidation;
using Modules.Orders.Application.Commands;

namespace Modules.Orders.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(c => c.Items).NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(c => c.Items)
            .ChildRules(items =>
            {
                items
                    .RuleFor(i => i.ProductName)
                    .NotEmpty()
                    .WithMessage("Product name is required.");

                items
                    .RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero.");

                items
                    .RuleFor(i => i.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than zero.");
            });
    }
}
