using Exptour.Application.DTOs.Category;
using FluentValidation;

namespace Exptour.Application.Validators.Category;

public class CategoryDTOValidator : AbstractValidator<CategoryDTO>
{
    public CategoryDTOValidator()
    {
        RuleFor(x => x.NameEN)
            .NotNull()
            .NotEmpty()
            .WithMessage("The name is required");

        RuleFor(x => x.NameAR)
            .NotNull()
            .NotEmpty()
            .WithMessage("The name is required");
    }
}
