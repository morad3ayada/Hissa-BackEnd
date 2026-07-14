using FluentValidation;
using LearningPlatform.Application.Features.Lessons.Commands;

namespace LearningPlatform.Application.Features.Lessons.Validators;

public class UpdateLessonCommandValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.Content)
            .MaximumLength(20000);

        RuleFor(x => x.DurationInSeconds)
            .GreaterThan(0)
            .When(x => x.DurationInSeconds.HasValue);

        RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
    }
}
