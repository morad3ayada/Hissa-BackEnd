using FluentValidation;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Application.Features.Lessons.Validators;

public class ReplaceVideoCommandValidator : AbstractValidator<ReplaceVideoCommand>
{
    public ReplaceVideoCommandValidator(IOptions<FileStorageSettings> fileStorageSettings)
    {
        var settings = fileStorageSettings.Value;

        RuleFor(x => x.LessonId).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(fileName => settings.AllowedVideoExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
            .WithMessage($"Video must be one of: {string.Join(", ", settings.AllowedVideoExtensions)}.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(settings.MaxVideoSizeInBytes)
            .WithMessage($"Video must not exceed {settings.MaxVideoSizeInBytes / 1024 / 1024} MB.");

        RuleFor(x => x.DurationInSeconds)
            .GreaterThan(0)
            .When(x => x.DurationInSeconds.HasValue);
    }
}
