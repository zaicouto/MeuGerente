using FluentValidation;
using Modules.Orders.Application.Commands;

namespace Modules.Orders.Application.Validators;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("O ID do pedido é obrigatório.")
            .Length(24)
            .WithMessage("O ID do pedido deve ter exatamente 24 caracteres.");

        RuleFor(x => x.Status).IsInEnum().WithMessage("Status do pedido inválido.");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("A lista de itens não pode ser nula.")
            .Must(items => items.Count != 0)
            .WithMessage("O pedido deve conter ao menos um item.");

        RuleForEach(x => x.Items)
            .ChildRules(items =>
            {
                items
                    .RuleFor(i => i.ProductName)
                    .NotEmpty()
                    .WithMessage("O nome do produto é obrigatório.");

                items
                    .RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("A quantidade do item deve ser maior que zero.");
            });
    }
}
