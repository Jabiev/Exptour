using Exptour.Application.DTOs.User;
using Exptour.Common.Helpers;
using FluentValidation;

namespace Exptour.Application.Validators.User;

public class UpdatePasswordDTOValidator : AbstractValidator<UpdatePasswordDTO>
{
    public UpdatePasswordDTOValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required")
            .Must(userId => userId.IsValidGuid()).WithMessage("UserId is not valid");
        RuleFor(x => x.ResetToken)
            .NotEmpty()
            .WithMessage("ResetToken is required")
            .Must(resetToken => resetToken.IsBase64UrlValid()).WithMessage("ResetToken is not valid");
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("NewPassword is required");
    }
}
