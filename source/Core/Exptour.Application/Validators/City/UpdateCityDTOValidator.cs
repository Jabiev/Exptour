using Exptour.Application.DTOs.City;
using FluentValidation;

namespace Exptour.Application.Validators.City;

public class UpdateCityDTOValidator : AbstractValidator<UpdateCityDTO>
{
    public UpdateCityDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name is required");
    }
}
