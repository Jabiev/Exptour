using Exptour.Application.DTOs.User;
using FluentValidation;

namespace Exptour.Application.Validators.User;

public class PasswordResetDTOValidator : AbstractValidator<PasswordResetDTO>
{
    public PasswordResetDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is not valid");
    }
}
