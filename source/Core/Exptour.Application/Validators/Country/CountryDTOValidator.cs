using Exptour.Application.DTOs.Country;
using FluentValidation;

namespace Exptour.Application.Validators.Country;

public class CountryDTOValidator : AbstractValidator<CountryDTO>
{
    public CountryDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull().WithMessage("Name is required");
    }
}
