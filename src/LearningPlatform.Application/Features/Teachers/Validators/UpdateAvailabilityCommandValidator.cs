using FluentValidation;

namespace LearningPlatform.Application.Features.Teachers.Validators;

public class UpdateAvailabilityCommandValidator : AbstractValidator<Commands.UpdateAvailabilityCommand>
{
    private static readonly HashSet<string> ValidDays =
        new(StringComparer.OrdinalIgnoreCase) { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    public UpdateAvailabilityCommandValidator()
    {
        RuleFor(x => x.Availability)
            .NotEmpty()
            .WithMessage("At least one availability slot is required.");

        RuleForEach(x => x.Availability).ChildRules(slot =>
        {
            slot.RuleFor(s => s.Day)
                .NotEmpty()
                .Must(d => ValidDays.Contains(d))
                .WithMessage("Invalid day name.");

            slot.RuleFor(s => s.StartTime)
                .NotEmpty()
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("StartTime must be in HH:mm format.");

            slot.RuleFor(s => s.EndTime)
                .NotEmpty()
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("EndTime must be in HH:mm format.");
        });
    }
}
