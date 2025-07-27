using FluentValidation;
using Modules.Users.Application.Commands;

namespace Modules.Users.Application.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithMessage("O primeiro nome é obrigatório.")
            .MaximumLength(50)
            .WithMessage("O primeiro nome deve ter no máximo 50 caracteres.");

        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithMessage("O sobrenome é obrigatório.")
            .MaximumLength(50)
            .WithMessage("O sobrenome deve ter no máximo 50 caracteres.");

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado não é válido.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres.");
    }
}
