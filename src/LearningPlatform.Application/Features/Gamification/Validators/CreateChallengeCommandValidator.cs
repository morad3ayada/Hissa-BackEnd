using FluentValidation;
using LearningPlatform.Application.Features.Gamification.Commands;

namespace LearningPlatform.Application.Features.Gamification.Validators;

public class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(x => x.OpponentId).NotEmpty();
        RuleFor(x => x.QuizId).NotEmpty();

        RuleFor(x => x.DurationInMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(7 * 24 * 60)
            .WithMessage("Challenge duration must be between 1 minute and 7 days.");

        RuleFor(x => x.Title)
            .MaximumLength(200);
    }
}
