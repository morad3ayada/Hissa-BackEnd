using FluentValidation;

namespace LearningPlatform.Application.Features.Subscriptions.Validators;

public class SubscribeCommandValidator : AbstractValidator<Commands.SubscribeCommand>
{
    public SubscribeCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty();
    }
}
