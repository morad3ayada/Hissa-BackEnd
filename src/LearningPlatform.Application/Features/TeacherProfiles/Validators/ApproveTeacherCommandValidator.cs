using FluentValidation;

namespace LearningPlatform.Application.Features.TeacherProfiles.Validators;

public class ApproveTeacherCommandValidator : AbstractValidator<Commands.ApproveTeacherCommand>
{
    public ApproveTeacherCommandValidator()
    {
        RuleFor(x => x.TeacherProfileId).NotEmpty();
    }
}
