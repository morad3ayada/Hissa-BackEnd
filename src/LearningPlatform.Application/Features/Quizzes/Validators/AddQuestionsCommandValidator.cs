using FluentValidation;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Quizzes.Validators;

public class AddQuestionsCommandValidator : AbstractValidator<AddQuestionsCommand>
{
    private static readonly string[] AllowedTypes =
        Enum.GetNames<QuestionType>();

    public AddQuestionsCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();

        RuleFor(x => x.Questions)
            .NotEmpty()
            .WithMessage("At least one question is required.");

        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(x => x.Text)
                .NotEmpty()
                .MaximumLength(2000);

            q.RuleFor(x => x.Explanation)
                .MaximumLength(2000);

            q.RuleFor(x => x.Points)
                .GreaterThan(0);

            q.RuleFor(x => x.Type)
                .Must(t => AllowedTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Type must be one of: {string.Join(", ", AllowedTypes)}.");

            // أسئلة Answers مطلوبة لكل أنواع إلا ShortAnswer/Essay
            q.RuleFor(x => x.Answers)
                .Must(a => a.Count == 4)
                .WithMessage("SingleChoice / MultipleChoice questions require exactly 4 answers.")
                .When(x => x.Type is "SingleChoice" or "MultipleChoice");

            q.RuleFor(x => x.Answers)
                .Must(a => a.Count == 2)
                .WithMessage("TrueFalse questions require exactly 2 answers.")
                .When(x => x.Type == "TrueFalse");

            // يجب أن يكون في إجابة صحيحة واحدة على الأقل
            q.RuleFor(x => x.Answers)
                .Must(a => a.Any(o => o.IsCorrect))
                .WithMessage("At least one answer must be marked as correct.")
                .When(x => x.Answers.Count > 0);

            // SingleChoice / TrueFalse: إجابة صحيحة واحدة بالضبط
            q.RuleFor(x => x.Answers)
                .Must(a => a.Count(o => o.IsCorrect) == 1)
                .WithMessage("Exactly one answer must be marked as correct for SingleChoice / TrueFalse.")
                .When(x => x.Type is "SingleChoice" or "TrueFalse");

            q.RuleForEach(x => x.Answers).ChildRules(a =>
            {
                a.RuleFor(x => x.Text)
                    .NotEmpty()
                    .MaximumLength(500);
            });
        });
    }
}
