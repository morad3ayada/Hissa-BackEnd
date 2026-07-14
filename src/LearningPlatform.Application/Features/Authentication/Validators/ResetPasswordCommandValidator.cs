using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.NewPassword).MustBeAStrongPassword();
    }
}
