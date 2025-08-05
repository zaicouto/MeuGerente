using FluentValidation;
using Modules.Orders.Application.Commands;

namespace Modules.Orders.Application.Validators;

public class SoftDeleteOrderCommandValidator : AbstractValidator<SoftDeleteOrderCommand>
{
    public SoftDeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("O ID do pedido é obrigatório.")
            .Length(24)
            .WithMessage("O ID do pedido deve ter exatamente 24 caracteres.");
    }
}
