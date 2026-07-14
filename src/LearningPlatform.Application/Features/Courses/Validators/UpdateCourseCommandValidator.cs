using FluentValidation;
using LearningPlatform.Application.Features.Courses.Commands;

namespace LearningPlatform.Application.Features.Courses.Validators;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Category)
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountPrice)
            .GreaterThanOrEqualTo(0)
            .LessThan(x => x.Price)
            .WithMessage("Discount price must be lower than the regular price.")
            .When(x => x.DiscountPrice.HasValue);

        RuleFor(x => x.Level).IsInEnum();

        RuleFor(x => x.Language)
            .NotEmpty()
            .MaximumLength(10);
    }
}
