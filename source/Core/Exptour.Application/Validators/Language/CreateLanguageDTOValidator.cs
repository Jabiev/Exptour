using Exptour.Application.DTOs.Language;
using FluentValidation;

namespace Exptour.Application.Validators.Language;

public class CreateLanguageDTOValidator : AbstractValidator<CreateLanguageDTO>
{
    public CreateLanguageDTOValidator()
    {
        RuleFor(x => x.NameEN)
            .NotNull()
            .NotEmpty()
            .WithMessage("NameEN is required.");
    }
}
