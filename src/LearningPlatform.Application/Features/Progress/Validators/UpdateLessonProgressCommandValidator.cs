using FluentValidation;
using LearningPlatform.Application.Features.Progress.Commands;

namespace LearningPlatform.Application.Features.Progress.Validators;

public class UpdateLessonProgressCommandValidator : AbstractValidator<UpdateLessonProgressCommand>
{
    public UpdateLessonProgressCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();

        RuleFor(x => x.CurrentSecond).GreaterThanOrEqualTo(0);

        RuleFor(x => x.WatchPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Watch percentage must be between 0 and 100.");
    }
}
