using FluentValidation;
using LearningPlatform.Application.Features.CourseSections.Commands;

namespace LearningPlatform.Application.Features.CourseSections.Validators;

public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
