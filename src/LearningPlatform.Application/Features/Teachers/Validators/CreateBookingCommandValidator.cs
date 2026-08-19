using FluentValidation;

namespace LearningPlatform.Application.Features.Teachers.Validators;

public class CreateBookingCommandValidator : AbstractValidator<Commands.CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.TeacherId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Date must be in YYYY-MM-DD format.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .Matches(@"^\d{2}:\d{2}$")
            .WithMessage("StartTime must be in HH:mm format.");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DurationInMinutes)
            .InclusiveBetween(15, 480)
            .WithMessage("Duration must be between 15 and 480 minutes.");
    }
}
