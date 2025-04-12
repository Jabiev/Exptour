using Exptour.Application.DTOs.Auth;
using FluentValidation;

namespace Exptour.Application.Validators.Auth;

public class VerifyOTPDTOValidator : AbstractValidator<VerifyOTPDTO>
{
    public VerifyOTPDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .NotNull()
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.OTPCode)
            .NotEmpty().WithMessage("OTP Code is required.")
            .NotNull()
            .Length(6).WithMessage("OTP Code must be 6 characters.");
    }
}
