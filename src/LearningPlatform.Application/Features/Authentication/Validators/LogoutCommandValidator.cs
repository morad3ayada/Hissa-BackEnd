using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
