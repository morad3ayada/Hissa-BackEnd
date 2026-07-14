using FluentValidation;
using LearningPlatform.Application.Features.LiveSessions.Commands;

namespace LearningPlatform.Application.Features.LiveSessions.Validators;

public class CreateLiveSessionCommandValidator : AbstractValidator<CreateLiveSessionCommand>
{
    public CreateLiveSessionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.CourseId).NotEmpty();

        RuleFor(x => x.MeetingPlatform).IsInEnum();

        RuleFor(x => x.MeetingLink)
            .NotEmpty()
            .MaximumLength(1000)
            .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _))
            .WithMessage("MeetingLink must be a valid absolute URL.");

        RuleFor(x => x.MeetingPassword)
            .MaximumLength(100);

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Session start time cannot be in the past.");

        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("Session end time must be after the start time.");
    }
}
