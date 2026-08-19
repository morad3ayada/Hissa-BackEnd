using FluentValidation;

namespace LearningPlatform.Application.Features.TeacherProfiles.Validators;

public class RejectTeacherCommandValidator : AbstractValidator<Commands.RejectTeacherCommand>
{
    public RejectTeacherCommandValidator()
    {
        RuleFor(x => x.TeacherProfileId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000);
    }
}
