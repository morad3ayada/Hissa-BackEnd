using FluentValidation;

namespace LearningPlatform.Application.Features.Teachers.Validators;

public class CreateNoteCommandValidator : AbstractValidator<Commands.CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty();

        RuleFor(x => x.Note)
            .NotEmpty()
            .MaximumLength(4000);
    }
}
