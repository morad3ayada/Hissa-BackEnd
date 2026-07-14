using FluentValidation;
using LearningPlatform.Application.Features.Courses.Commands;

namespace LearningPlatform.Application.Features.Courses.Validators;

public class SetCourseStateCommandValidator : AbstractValidator<SetCourseStateCommand>
{
    public SetCourseStateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
