using FluentValidation;
using LearningPlatform.Application.Features.CourseSections.Commands;

namespace LearningPlatform.Application.Features.CourseSections.Validators;

public class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
    }
}
