using FluentValidation;
using LearningPlatform.Application.Features.Payments.Commands;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Application.Features.Payments.Validators;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator(IOptions<FileStorageSettings> fileStorageSettings)
    {
        var settings = fileStorageSettings.Value;

        RuleFor(x => x.CourseId).NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.TransactionNumber)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Notes)
            .MaximumLength(1000);

        RuleFor(x => x.ReceiptImageFileName)
            .NotEmpty()
            .Must(fileName => settings.AllowedImageExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
            .WithMessage($"Receipt image must be one of: {string.Join(", ", settings.AllowedImageExtensions)}.");

        RuleFor(x => x.ReceiptImageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(settings.MaxImageSizeInBytes)
            .WithMessage($"Receipt image must not exceed {settings.MaxImageSizeInBytes / 1024 / 1024} MB.");
    }
}
