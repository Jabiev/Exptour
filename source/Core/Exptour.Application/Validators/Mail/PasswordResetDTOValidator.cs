using Exptour.Application.DTOs.Mail;
using FluentValidation;

namespace Exptour.Application.Validators.Mail;

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
