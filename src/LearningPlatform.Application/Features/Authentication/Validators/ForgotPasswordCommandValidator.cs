using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
