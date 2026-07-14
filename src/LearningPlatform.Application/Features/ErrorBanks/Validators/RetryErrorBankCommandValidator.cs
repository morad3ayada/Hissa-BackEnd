using FluentValidation;
using LearningPlatform.Application.Features.ErrorBanks.Commands;

namespace LearningPlatform.Application.Features.ErrorBanks.Validators;

public class RetryErrorBankCommandValidator : AbstractValidator<RetryErrorBankCommand>
{
    public RetryErrorBankCommandValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("At least one answer must be submitted.");

        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.QuestionId).NotEmpty();
        });

        RuleFor(x => x.Answers)
            .Must(a => a.Select(x => x.QuestionId).Distinct().Count() == a.Count)
            .WithMessage("Duplicate question answers are not allowed in a single retry submission.")
            .When(x => x.Answers.Count > 0);
    }
}
