using Exptour.Application.DTOs.User;
using Exptour.Common.Helpers;
using FluentValidation;

namespace Exptour.Application.Validators.User;

public class AssignRoleDTOValidator : AbstractValidator<AssignRoleDTO>
{
    public AssignRoleDTOValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required")
            .Must(userId => userId.IsValidGuid()).WithMessage("UserId is not valid");
        RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("At least one role must be assigned.")
            .Must(roles => roles.All(role => !string.IsNullOrWhiteSpace(role)))
            .WithMessage("Roles cannot contain empty or whitespace values.");
    }
}
