using FluentValidation;
using LearningPlatform.Application.Features.Payments.Commands;

namespace LearningPlatform.Application.Features.Payments.Validators;

public class RejectPaymentCommandValidator : AbstractValidator<RejectPaymentCommand>
{
    public RejectPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
