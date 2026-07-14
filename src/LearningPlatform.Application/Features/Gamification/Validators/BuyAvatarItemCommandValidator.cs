using FluentValidation;
using LearningPlatform.Application.Features.Gamification.Commands;

namespace LearningPlatform.Application.Features.Gamification.Validators;

public class BuyAvatarItemCommandValidator : AbstractValidator<BuyAvatarItemCommand>
{
    public BuyAvatarItemCommandValidator()
    {
        RuleFor(x => x.AvatarItemId).NotEmpty();
    }
}
