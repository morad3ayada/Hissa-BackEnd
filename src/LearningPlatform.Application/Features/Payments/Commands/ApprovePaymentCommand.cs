using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Payments.Commands;

public record ApprovePaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public Guid PaymentId { get; init; }
}
