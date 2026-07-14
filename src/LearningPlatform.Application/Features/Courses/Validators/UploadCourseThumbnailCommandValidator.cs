using FluentValidation;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Application.Features.Courses.Validators;

public class UploadCourseThumbnailCommandValidator : AbstractValidator<UploadCourseThumbnailCommand>
{
    public UploadCourseThumbnailCommandValidator(IOptions<FileStorageSettings> fileStorageSettings)
    {
        var settings = fileStorageSettings.Value;

        RuleFor(x => x.CourseId).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(fileName => settings.AllowedImageExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
            .WithMessage($"Thumbnail must be one of: {string.Join(", ", settings.AllowedImageExtensions)}.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(settings.MaxImageSizeInBytes)
            .WithMessage($"Thumbnail must not exceed {settings.MaxImageSizeInBytes / 1024 / 1024} MB.");
    }
}
