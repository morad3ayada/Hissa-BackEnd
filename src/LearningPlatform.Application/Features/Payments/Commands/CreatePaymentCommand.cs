using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Payments.Commands;

public record CreatePaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public Guid CourseId { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string TransactionNumber { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public Stream ReceiptImageStream { get; init; } = Stream.Null;
    public string ReceiptImageFileName { get; init; } = string.Empty;
    public string ReceiptImageContentType { get; init; } = string.Empty;
    public long ReceiptImageSize { get; init; }
}
