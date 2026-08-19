using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class VerifyOtpCommandValidator : AbstractValidator<Commands.VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Otp)
            .NotEmpty()
            .Matches(@"^\d{6}$").WithMessage("OTP must be exactly 6 digits.");
    }
}
