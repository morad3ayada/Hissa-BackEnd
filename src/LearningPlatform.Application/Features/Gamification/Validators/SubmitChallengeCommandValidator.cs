using FluentValidation;
using LearningPlatform.Application.Features.Gamification.Commands;

namespace LearningPlatform.Application.Features.Gamification.Validators;

public class SubmitChallengeCommandValidator : AbstractValidator<SubmitChallengeCommand>
{
    public SubmitChallengeCommandValidator()
    {
        RuleFor(x => x.ChallengeId).NotEmpty();

        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("At least one answer must be submitted.");

        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.QuestionId).NotEmpty();
        });

        RuleFor(x => x.Answers)
            .Must(a => a.Select(x => x.QuestionId).Distinct().Count() == a.Count)
            .WithMessage("Duplicate question answers are not allowed in a single submission.")
            .When(x => x.Answers.Count > 0);
    }
}
