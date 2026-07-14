using FluentValidation;
using LearningPlatform.Application.Features.Quizzes.Commands;

namespace LearningPlatform.Application.Features.Quizzes.Validators;

public class SubmitQuizCommandValidator : AbstractValidator<SubmitQuizCommand>
{
    public SubmitQuizCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();

        RuleFor(x => x.StartedAt)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
            .WithMessage("StartedAt cannot be in the future.");

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
