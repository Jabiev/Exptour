using Exptour.Application.DTOs.City;
using Exptour.Common.Helpers;
using FluentValidation;

namespace Exptour.Application.Validators.City;

public class CityDTOValidator : AbstractValidator<CityDTO>
{
    public CityDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name is required");

        RuleFor(x => x.CountryId)
            .NotEmpty()
            .NotNull().WithMessage("Country is required")
            .Must(countryId => countryId.IsValidGuid()).WithMessage("CountryId is not valid");
    }
}
