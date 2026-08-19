using FluentValidation;

namespace LearningPlatform.Application.Features.Teachers.Validators;

public class CreateUnavailableSlotCommandValidator : AbstractValidator<Commands.CreateUnavailableSlotCommand>
{
    public CreateUnavailableSlotCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Date must be in YYYY-MM-DD format.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .Matches(@"^\d{2}:\d{2}$")
            .WithMessage("StartTime must be in HH:mm format.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .Matches(@"^\d{2}:\d{2}$")
            .WithMessage("EndTime must be in HH:mm format.");
    }
}
