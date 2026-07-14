using FluentValidation;
using LearningPlatform.Application.Features.Enrollments.Commands;

namespace LearningPlatform.Application.Features.Enrollments.Validators;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
