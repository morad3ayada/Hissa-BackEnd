using FluentValidation;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Quizzes.Validators;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Scope).IsInEnum();

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("CourseId is required for a course-scoped quiz.")
            .When(x => x.Scope == QuizScope.Course);

        RuleFor(x => x.LessonId)
            .NotEmpty().WithMessage("LessonId is required for a lesson-scoped quiz.")
            .When(x => x.Scope == QuizScope.Lesson);

        RuleFor(x => x.LessonId)
            .Empty().WithMessage("LessonId must not be set for a course-scoped quiz.")
            .When(x => x.Scope == QuizScope.Course);

        RuleFor(x => x.CourseId)
            .Empty().WithMessage("CourseId must not be set for a lesson-scoped quiz.")
            .When(x => x.Scope == QuizScope.Lesson);

        RuleFor(x => x.IsFinalExam)
            .Equal(false).WithMessage("Only a course-scoped quiz can be the final exam.")
            .When(x => x.Scope != QuizScope.Course);

        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);

        RuleFor(x => x.TimeLimitInMinutes)
            .GreaterThan(0)
            .When(x => x.TimeLimitInMinutes.HasValue);

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0)
            .When(x => x.MaxAttempts.HasValue);
    }
}
