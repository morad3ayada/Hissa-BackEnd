using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class SendOtpCommandValidator : AbstractValidator<Commands.SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);
    }
}
