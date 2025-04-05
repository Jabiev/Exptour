using Exptour.Application.DTOs.Guide;
using FluentValidation;

namespace Exptour.Application.Validators.Guide;

public class CreateGuideDTOValidator : AbstractValidator<CreateGuideDTO>
{
    public CreateGuideDTOValidator()
    {
        RuleFor(x => x.FullName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Full Name is required.");

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.LanguageIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least 2 languages are required.");
    }
}
