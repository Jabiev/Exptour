using Exptour.Application.DTOs.Auth;
using FluentValidation;

namespace Exptour.Application.Validators.Auth;

public class SignInDTOValidator : AbstractValidator<SignInDTO>
{
    public SignInDTOValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .MaximumLength(255)
            .NotEmpty().WithMessage("User Name or Email is required.")
            .NotNull();

        RuleFor(x => x.Password)
            .MaximumLength(255)
            .NotEmpty().WithMessage("Password is required.")
            .NotNull();
    }
}
