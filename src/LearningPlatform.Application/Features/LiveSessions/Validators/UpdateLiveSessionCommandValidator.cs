using FluentValidation;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.LiveSessions.Validators;

public class UpdateLiveSessionCommandValidator : AbstractValidator<UpdateLiveSessionCommand>
{
    public UpdateLiveSessionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.MeetingPlatform).IsInEnum();

        RuleFor(x => x.MeetingLink)
            .NotEmpty()
            .MaximumLength(1000)
            .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _))
            .WithMessage("MeetingLink must be a valid absolute URL.");

        RuleFor(x => x.MeetingPassword)
            .MaximumLength(100);

        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("Session end time must be after the start time.");

        RuleFor(x => x.Status)
            .Must(s => s is null or LiveSessionStatus.Scheduled or LiveSessionStatus.Cancelled)
            .WithMessage("Status can only be set to Scheduled or Cancelled; Completed is derived automatically.");
    }
}
