using MediatR;

namespace LearningPlatform.Application.Features.Payments.Queries;

public record GetPaymentReceiptQuery(Guid PaymentId) : IRequest<PaymentReceiptResult>;

public record PaymentReceiptResult(Stream Stream, string ContentType, string FileName);
