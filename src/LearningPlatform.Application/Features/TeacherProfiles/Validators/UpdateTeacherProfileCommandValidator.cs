using FluentValidation;

namespace LearningPlatform.Application.Features.TeacherProfiles.Validators;

public class UpdateTeacherProfileCommandValidator : AbstractValidator<Commands.UpdateTeacherProfileCommand>
{
    public UpdateTeacherProfileCommandValidator()
    {
        RuleFor(x => x.RealName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Specialization)
            .MaximumLength(200)
            .When(x => x.Specialization is not null);

        RuleFor(x => x.Governorate)
            .MaximumLength(100)
            .When(x => x.Governorate is not null);

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .When(x => x.Bio is not null);

        RuleFor(x => x.LessonPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LessonPrice is not null);

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0)
            .When(x => x.YearsOfExperience is not null);

        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500)
            .When(x => x.ProfileImageUrl is not null);
    }
}
