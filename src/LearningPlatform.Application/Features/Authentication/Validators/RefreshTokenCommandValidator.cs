using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
