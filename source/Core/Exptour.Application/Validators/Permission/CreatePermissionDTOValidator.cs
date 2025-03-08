using Exptour.Application.DTOs.Permission;
using FluentValidation;

namespace Exptour.Application.Validators.Permission;

public class CreatePermissionDTOValidator : AbstractValidator<CreatePermissionDTO>
{
    public CreatePermissionDTOValidator()
    {
        RuleFor(x => x.NameEN)
            .NotEmpty()
            .NotNull()
            .WithMessage("UserId is required");
    }
}
