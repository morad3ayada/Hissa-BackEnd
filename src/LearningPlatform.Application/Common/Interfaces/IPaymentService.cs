namespace LearningPlatform.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken = default);

    Task<PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default);
}

public record PaymentRequest(decimal Amount, string Currency, string CustomerReference, string Description);

public record PaymentResult(bool Succeeded, string? TransactionId, string? ErrorMessage);
