using FluentValidation;
using LearningPlatform.Application.Features.Quizzes.Commands;

namespace LearningPlatform.Application.Features.Quizzes.Validators;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Explanation)
            .MaximumLength(2000);

        RuleFor(x => x.Points)
            .GreaterThan(0);

        RuleFor(x => x.Answers)
            .Must(a => a.Count == 4)
            .WithMessage("A multiple-choice question requires exactly 4 answers.");

        RuleFor(x => x.Answers)
            .Must(a => a.Count(o => o.IsCorrect) == 1)
            .WithMessage("Exactly one answer must be marked as correct.")
            .When(x => x.Answers.Count > 0);

        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.Id).NotEmpty();
            answer.RuleFor(a => a.Text)
                .NotEmpty()
                .MaximumLength(500);
        });
    }
}
