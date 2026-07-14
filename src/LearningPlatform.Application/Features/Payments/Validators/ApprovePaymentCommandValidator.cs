using FluentValidation;
using LearningPlatform.Application.Features.Payments.Commands;

namespace LearningPlatform.Application.Features.Payments.Validators;

public class ApprovePaymentCommandValidator : AbstractValidator<ApprovePaymentCommand>
{
    public ApprovePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}
